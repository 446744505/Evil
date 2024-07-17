using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Evil.Util;

namespace Edb
{
    public sealed class Logs
    {
        private Logs() {}

        public static void LogObject(XBean xBean, string varName)
        {
            var key = new LogKey(xBean, varName);
            var sp = Transaction.CurrentSavepoint;
            if (sp.Get(key) == null)
                sp.Add(key, new LogObject(key));
        }

        public static IList<T> LogList<T>(XBean xBean, string varName, Action verify)
        {
            var key = new LogKey(xBean, varName);
            var wrappers = Transaction.Current!.Wrappers;
            if (!wrappers.TryGetValue(key, out var log))
            {
                wrappers[key] = log = new LogList<T>(key, (key.Value as IList<T>)!);
            }

            return ((LogList<T>)log).SetVerify(verify);
        }
        
        public static ISet<T> LogSet<T>(XBean xBean, string varName, Action verify)
        {
            var key = new LogKey(xBean, varName);
            var wrappers = Transaction.Current!.Wrappers;
            if (!wrappers.TryGetValue(key, out var log))
            {
                wrappers[key] = log = new LogSet<T>(key, (key.Value as ISet<T>)!);
            }
         
            return ((LogSet<T>)log).SetVerify(verify);
        }
        
        public static IDictionary<TKey, TValue> LogMap<TKey, TValue>(XBean xBean, string varName, Action verify) where TKey : notnull
        {
            var key = new LogKey(xBean, varName);
            var wrappers = Transaction.Current!.Wrappers;
            if (!wrappers.TryGetValue(key, out var log))
            {
                wrappers[key] = log = new LogMap<TKey, TValue, IDictionary<TKey, TValue>>(key, (key.Value as IDictionary<TKey, TValue>)!);
            }
          
            return ((LogMap<TKey, TValue, IDictionary<TKey, TValue>>)log).SetVerify(verify);
        }

        internal static void Link(object? bean, XBean? parent, string? varName, bool log = true)
        {
            switch (bean)
            {
                case null:
                    throw new NullReferenceException();
                case XBean xBean:
                    xBean.Link(parent, varName, log);
                    break;
            }
        }
    }

    internal sealed class LogObject : INote, ILog
    {
        private readonly LogKey m_LogKey;
        private readonly object m_Origin;
        
        internal LogObject(LogKey logKey)
        {
            m_LogKey = logKey;
            m_Origin = m_LogKey.Value;
        }
        
        public void Commit()
        {
            LogNotify.Notify(m_LogKey, this);
        }

        public void Rollback()
        {
            m_LogKey.Value = m_Origin;
        }

        public override string? ToString()
        {
            return m_Origin.ToString();
        }
    }

    internal class WrapList<T> : IList<T>
    {
        private readonly LogList<T>? m_Root;
        private readonly IList<T> m_Wrapped;
        
        protected IList<T> Wrapped => m_Wrapped;
        public int Count => m_Wrapped.Count;
        public bool IsReadOnly => m_Wrapped.IsReadOnly;
        
        protected WrapList(LogList<T>? root, IList<T> wrapped)
        {
            m_Root = root;
            m_Wrapped = wrapped;
        }

        protected virtual void BeforeChange()
        {
            m_Root!.BeforeChange();
        }

        protected virtual void AfterAdd(T item)
        {
            m_Root!.AfterAdd(item);
        }
        
        protected virtual void BeforeRemove(T item)
        {
            m_Root!.BeforeRemove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            // 不会修改原始set，无需包装
            return m_Wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            BeforeChange();
            m_Wrapped.Add(item);
            AfterAdd(item);
        }

        public void Clear()
        {
            BeforeChange();
            foreach (var e in m_Wrapped)
            {
                BeforeRemove(e);
            }
            m_Wrapped.Clear();
        }

        public bool Contains(T item)
        {
            return m_Wrapped.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_Wrapped.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var idx = IndexOf(item);
            if (idx < 0)
                return false;
            RemoveAt(idx);
            return true;
        }
        
        public int IndexOf(T item)
        {
            return m_Wrapped.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            BeforeChange();
            m_Wrapped.Insert(index, item);
            AfterAdd(item);
        }

        public void RemoveAt(int index)
        {
            BeforeChange();
            BeforeRemove(m_Wrapped[index]);
            m_Wrapped.RemoveAt(index);
        }

        public T this[int index]
        {
            get => m_Wrapped[index];
            set
            {
                BeforeChange();
                BeforeRemove(m_Wrapped[index]);
                m_Wrapped[index] = value;
                AfterAdd(value);
            }
        }

        public override int GetHashCode()
        {
            return m_Wrapped.GetHashCode();
        }
    }

    internal class LogList<T> : WrapList<T>
    {
        private readonly LogKey m_LogKey;
        private Action m_Verify = null!;

        public LogList(LogKey logKey, IList<T> wrapped) : base(null, wrapped)
        {
            m_LogKey = logKey;
        }

        private MyLog GetOrCreateMyLog()
        {
            var sp = Transaction.CurrentSavepoint;
            var log = sp.Get(m_LogKey);
            if (log != null) 
                return (MyLog)log;
            
            log = new MyLog(this);
            sp.Add(m_LogKey, log);

            return (MyLog)log;
        }

        protected override void BeforeChange()
        {
            m_Verify.Invoke();
            GetOrCreateMyLog().BeforeChange();
        }
        
        protected override void AfterAdd(T item)
        {
            Logs.Link(item, m_LogKey.XBean, m_LogKey.VarName);
        }
        
        protected override void BeforeRemove(T item)
        {
            Logs.Link(item, null, null);
        }
        
        public LogList<T> SetVerify(Action verify)
        {
            m_Verify = verify;
            return this;
        }

        private sealed class MyLog : INote, ILog
        {
            private readonly LogList<T> m_LogList;
            private T[]? m_SavedOnWrite;

            public MyLog(LogList<T> logList)
            {
                m_LogList = logList;
            }

            public void Commit()
            {
                if (m_SavedOnWrite != null)
                    LogNotify.Notify(m_LogList.m_LogKey, this);
            }

            public void Rollback()
            {
                m_LogList.Wrapped.Clear();
                for (var i = 0; i < m_SavedOnWrite!.Length; i++)
                {
                    var e = m_SavedOnWrite[i];
                    m_LogList.Wrapped.Add(e);
                }
            }

            internal void BeforeChange()
            {
                m_SavedOnWrite ??= m_LogList.Wrapped.ToArray();
            }
        }
    }

    internal class LogSet<T> : ISet<T>
    {
        private readonly LogKey m_LogKey;
        private readonly ISet<T> m_Wrapped;
        private Action m_Verify = null!;

        public int Count => m_Wrapped.Count;
        public bool IsReadOnly => m_Wrapped.IsReadOnly;
        
        public LogSet(LogKey logKey, ISet<T> wrapped)
        {
            m_LogKey = logKey;
            m_Wrapped = wrapped;
        }

        public LogSet<T> SetVerify(Action verify)
        {
            m_Verify = verify;
            return this;
        }

        private MyLog GetOrCreateMyLog()
        {
            var sp = Transaction.CurrentSavepoint;
            var log = sp.Get(m_LogKey);
            if (log != null) 
                return (MyLog)log;
            
            log = new MyLog(this);
            sp.Add(m_LogKey, log);

            return (MyLog)log;
        }

        public IEnumerator<T> GetEnumerator()
        {
            // 不会修改原始set，无需包装
            return m_Wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool AddIfNotPresent(T item)
        {
            m_Verify.Invoke();
            if (m_Wrapped.Add(item))
            {
                GetOrCreateMyLog().AfterAdd(item);
                Logs.Link(item, m_LogKey.XBean, m_LogKey.VarName);
                return true;
            }

            return false;
        }
        
        void ICollection<T>.Add(T item)
        {
            AddIfNotPresent(item);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            // 用的少，先不实现
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            // 用的少，先不实现
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return m_Wrapped.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return m_Wrapped.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return m_Wrapped.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return m_Wrapped.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return m_Wrapped.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return m_Wrapped.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            // 用的少，先不实现
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            // 用的少，先不实现
            throw new NotImplementedException();
        }

        bool ISet<T>.Add(T item)
        {
            return AddIfNotPresent(item);
        }

        public void Clear()
        {
            m_Verify.Invoke();
            var myLog = GetOrCreateMyLog();
            foreach (var e in m_Wrapped)
                myLog.BeforeRemove(e);
            m_Wrapped.Clear();
        }

        public bool Contains(T item)
        {
            return m_Wrapped.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_Wrapped.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            m_Verify.Invoke();
            if (!m_Wrapped.Remove(item)) 
                return false;
            
            GetOrCreateMyLog().AfterRemove(item);
            return true;

        }
        
        public override int GetHashCode()
        {
            return m_Wrapped.GetHashCode();
        }

        private class MyLog : NoteSet, ILog
        {
            private readonly LogSet<T> m_LogSet;

            public MyLog(LogSet<T> logSet)
            {
                m_LogSet = logSet;
            }

            public void Commit()
            {
                if (IsSetChanged)
                    LogNotify.Notify(m_LogSet.m_LogKey, this);
            }

            public void Rollback()
            {
                var wrapped = m_LogSet.m_Wrapped;
                foreach (var e in Added)
                    wrapped.Remove(e);
                foreach (var e in Removed)
                    wrapped.Add(e);
                foreach (var e in Eldest)
                    wrapped.Remove(e);
                foreach (var e in Eldest)
                    wrapped.Add(e);
                Clear();
            }

            public void BeforeRemove(T item)
            {
                Logs.Link(item, null, null);
                LogRemove(item);
            }

            public void AfterRemove(T item)
            {
                LogRemove(item);
                Logs.Link(item, null, null);
            }

            public void AfterAdd(T item)
            {
                LogAdd(item);
            }
        }

        private class NoteSet : INote
        {
            private readonly HashSet<T> m_Added = new();
            private readonly HashSet<T> m_Removed = new();
            private readonly HashSet<T> m_Eldest = new();
            
            protected ISet<T> Added => m_Added;
            protected ISet<T> Removed => m_Removed;
            protected ISet<T> Eldest => m_Eldest;
            protected bool IsSetChanged => m_Added.Count > 0 || m_Removed.Count > 0;

            void Merge(INote note)
            {
                var other = (NoteSet)note;
                foreach (var e in other.m_Added)
                    LogAdd(e);
                foreach (var e in other.m_Removed)
                    LogRemove(e);
            }
            
            protected void LogAdd(T item)
            {
                if (m_Removed.Remove(item))
                    return;
                
                m_Added.Add(item);
            }
            
            protected void LogRemove(T item)
            {
                if (m_Added.Remove(item))
                    return;
                
                m_Removed.Add(item);
                m_Eldest.Add(item);
            }

            protected void Clear()
            {
                m_Added.Clear();
                m_Removed.Clear();
                m_Eldest.Clear();
            }

            public override string ToString()
            {
                return $"added={m_Added} removed={m_Removed}";
            }
        }
    }

    internal class LogMap<TKey, TValue, TW> : IDictionary<TKey, TValue> where TW : IDictionary<TKey, TValue> where TKey : notnull
    {
        readonly LogKey m_LogKey;
        readonly TW m_Wrapped;
        private Action m_Verify = null!;
        
        public int Count => m_Wrapped.Count;
        public bool IsReadOnly => m_Wrapped.IsReadOnly;
        public ICollection<TKey> Keys => m_Wrapped.Keys; // 不会修改原始map，无需包装
        public ICollection<TValue> Values => m_Wrapped.Values; // 不会修改原始map，无需包装
        
        public LogMap(LogKey logKey, TW wrapped)
        {
            m_LogKey = logKey;
            m_Wrapped = wrapped;
        }
        
        public LogMap<TKey, TValue, TW> SetVerify(Action verify)
        {
            m_Verify = verify;
            return this;
        }
        
        private MyLog GetOrCreateMyLog()
        {
            var sp = Transaction.CurrentSavepoint;
            var log = sp.Get(m_LogKey);
            if (log != null) 
                return (MyLog)log;
            
            log = new MyLog(this);
            sp.Add(m_LogKey, log);

            return (MyLog)log;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            // 不会修改原始set，无需包装
            return m_Wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            m_Verify.Invoke();
            var myLog = GetOrCreateMyLog();
            foreach (var pair in m_Wrapped)
                myLog.BeforeRemove(pair.Key, pair.Value);
            m_Wrapped.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return m_Wrapped.Contains(pair);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            m_Wrapped.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            if (!m_Wrapped.TryGetValue(pair.Key, out var value) || !EqualityComparer<TValue>.Default.Equals(value, pair.Value))
                return false;
            return Remove(pair.Key);
        }
        
        public void Add(TKey key, TValue value)
        {
            m_Verify.Invoke();
            if (key == null || value == null)
                throw new NullReferenceException();
            Logs.Link(value, m_LogKey.XBean, m_LogKey.VarName);
            var hadOrigin = m_Wrapped.PutAndReturnValue(key, value, out var origin);
            GetOrCreateMyLog().AfterPut(key, origin, hadOrigin);
        }

        public bool ContainsKey(TKey key)
        {
            return m_Wrapped.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            m_Verify.Invoke();
            if (m_Wrapped.RemoveAndReturnValue(key, out var value))
            {
                GetOrCreateMyLog().AfterRemove(key, value!);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)]out TValue value)
        {
            return m_Wrapped.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => m_Wrapped[key];
            set
            {
                m_Verify.Invoke();
                if (key == null || value == null)
                    throw new NullReferenceException();
                Logs.Link(value, m_LogKey.XBean, m_LogKey.VarName);
                var hadOrigin = m_Wrapped.PutAndReturnValue(key, value, out var origin);
                GetOrCreateMyLog().AfterPut(key, origin, hadOrigin);
            }
        }

        public override string? ToString()
        {
            return m_Wrapped.ToString();
        }

        public override int GetHashCode()
        {
            return m_Wrapped.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return m_Wrapped.Equals(obj);
        }

        private class MyLog : NoteMap<TKey, TValue>, ILog
        {
            private readonly LogMap<TKey, TValue, TW> m_LogMap;

            public MyLog(LogMap<TKey, TValue, TW> logMap)
            {
                m_LogMap = logMap;
            }

            public void Commit()
            {
                if (IsMapChanged)
                    LogNotify.Notify(m_LogMap.m_LogKey, this);
            }

            public void Rollback()
            {
                var wrapped = m_LogMap.m_Wrapped;
                foreach (var k in Added)
                    wrapped.Remove(k);
                foreach (var pair in Removed)
                    wrapped.Add(pair.Key, pair.Value);
                foreach (var pair in Replaced)
                    wrapped[pair.Key] = pair.Value;
                Clear();
            }
            
            internal void BeforeRemove(TKey key, TValue value)
            {
                Logs.Link(value, null, null);
                LogRemove(key, value);
            }
            
            internal void AfterRemove(TKey key, TValue value)
            {
                LogRemove(key, value);
                Logs.Link(value, null, null);
            }
            
            public void AfterPut(TKey key, TValue? origin, bool hadOrigin)
            {
                LogPut(key, origin, hadOrigin);
                if (origin != null)
                    Logs.Link(origin, null, null);
            }
        }

        private class NoteMap<TKey, TValue> : INote where TKey : notnull
        {
            private readonly HashSet<TKey> m_Added = new();
            private readonly Dictionary<TKey, TValue> m_Removed = new();
            private readonly Dictionary<TKey, TValue> m_Replaced = new();
            private List<TValue>? m_Changed;
            private Dictionary<TKey, TValue>? m_ObjRef;
            private Dictionary<TKey, TValue>? m_ChangedMap;

            internal HashSet<TKey> Added => m_Added;
            internal Dictionary<TKey, TValue> Replaced => m_Replaced;
            public Dictionary<TKey, TValue> Removed => m_Removed;
            protected bool IsMapChanged => m_Added.Count > 0 || m_Removed.Count > 0 || m_Replaced.Count > 0;
            public Dictionary<TKey, TValue> Changed {
                get
                {
                    if (m_ChangedMap != null)
                        return m_ChangedMap;
                    
                    m_ChangedMap = m_Replaced.Keys.ToDictionary(k => k, k => m_ObjRef![k]);
                    if (m_Changed == null && m_Added.Count == 0)
                        return m_ChangedMap;
                    
                    foreach (var k in m_Added)
                        m_ChangedMap[k] = m_ObjRef![k];
                    if (m_Changed == null) 
                        return m_ChangedMap;
                    
                    var set = new HashSet<TValue>(new ReferenceEqualityComparer<TValue>());
                    foreach (var v in m_Changed)
                        set.Add(v);
                    m_ObjRef!.Where(pair => set.Contains(pair.Value)).ToList().ForEach(pair => m_ChangedMap[pair.Key] = pair.Value);

                    return m_ChangedMap;
                }
            }

            protected void SetChanged(List<TValue> changed, object objRef)
            {
                m_Changed = changed;
                m_ObjRef = (Dictionary<TKey, TValue>)objRef;
            }

            void Merge(INote note)
            {
                var other = (NoteMap<TKey, TValue>)note;
                foreach (var k in other.m_Added)
                    LogPut(k, default, false);
                foreach (var pair in other.m_Removed)
                    LogRemove(pair.Key, pair.Value);
                foreach (var pair in other.m_Replaced)
                    LogPut(pair.Key, pair.Value, true);
            }

            protected void LogRemove(TKey key, TValue value)
            {
                if (m_Added.Remove(key))
                    return;
                m_Replaced.RemoveAndReturnValue(key, out var v);
                m_Removed[key] = v == null ? value : v;
            }

            protected void LogPut(TKey key, TValue? origin, bool hadOrigin)
            {
                if (m_Added.Contains(key))
                    return;
                if (m_Removed.RemoveAndReturnValue(key, out var v))
                {
                    m_Replaced[key] = v!;
                    return;
                }
                if (m_Replaced.ContainsKey(key))
                    return;
                
                if (!hadOrigin)
                    m_Added.Add(key);
                else 
                    m_Replaced[key] = origin!;
            }

            protected void Clear()
            {
                m_Added.Clear();
                m_Removed.Clear();
                m_Replaced.Clear();
                m_Changed = null;
                m_ObjRef = null;
                m_ChangedMap = null;
            }

            public override string ToString()
            {
                return $"added={m_Added} removed={m_Removed} replaced={m_Replaced} changed={m_Changed}";
            }
        }
    }
}