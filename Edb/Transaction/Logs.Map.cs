using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Evil.Util;

namespace Edb
{
    public partial class Logs
    {
        public static IDictionary<TKey, TValue> LogMap<TKey, TValue>(XBean xBean, string varName, Action verify, TransactionCtx ctx) where TKey : notnull
        {
            var key = new LogKey(xBean, varName);
            var wrappers = ctx.Current!.Wrappers;
            if (!wrappers.TryGetValue(key, out var log))
            {
                wrappers[key] = log = new LogMap<TKey, TValue, Dictionary<TKey, TValue>>(key, (key.Value as Dictionary<TKey, TValue>)!);
            }
          
            return ((LogMap<TKey, TValue, Dictionary<TKey, TValue>>)log).SetVerify(verify);
        }
    }
    
    internal class LogMap<TKey, TValue, TW> : IDictionary<TKey, TValue> where TW : Dictionary<TKey, TValue> where TKey : notnull
    {
        readonly LogKey m_LogKey;
        readonly TW m_Wrapped;
        private Action m_Verify = null!;
        
        public int Count => m_Wrapped.Count;
        public bool IsReadOnly => false;
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
        
        private MyLog GetOrCreateMyLog(TransactionCtx ctx)
        {
            var sp = ctx.Current!.CurrentSavepoint;
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

        public void Add(KeyValuePair<TKey, TValue> pair, TransactionCtx ctx)
        {
            Add(pair.Key, pair.Value, ctx);
        }

        public void Clear(TransactionCtx ctx)
        {
            m_Verify();
            var myLog = GetOrCreateMyLog(ctx);
            foreach (var pair in m_Wrapped)
                myLog.BeforeRemove(pair.Key, pair.Value, ctx);
            m_Wrapped.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return m_Wrapped.Contains(pair);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var id = (IDictionary<TKey, TValue>)m_Wrapped;
            id.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair, TransactionCtx ctx)
        {
            if (!m_Wrapped.TryGetValue(pair.Key, out var value) || !EqualityComparer<TValue>.Default.Equals(value, pair.Value))
                return false;
            return Remove(pair.Key, ctx);
        }
        
        public void Add(TKey key, TValue value, TransactionCtx ctx)
        {
            m_Verify();
            if (key == null || value == null)
                throw new NullReferenceException();
            var hadOrigin = m_Wrapped.TryGetValue(key, out var origin);
            if (hadOrigin)
                throw new ArgumentException($"key={key} already exists in dictionary");
            Logs.Link(value, m_LogKey.XBean, m_LogKey.VarName, ctx);
            m_Wrapped[key] = value;
            GetOrCreateMyLog(ctx).AfterPut(key, origin, hadOrigin, ctx);
        }

        public bool ContainsKey(TKey key)
        {
            return m_Wrapped.ContainsKey(key);
        }

        public bool Remove(TKey key, TransactionCtx ctx)
        {
            m_Verify();
            if (m_Wrapped.Remove(key, out var value))
            {
                GetOrCreateMyLog(ctx).AfterRemove(key, value!, ctx);
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
                m_Verify();
                if (key == null || value == null)
                    throw new NullReferenceException();
                Logs.Link(value, m_LogKey.XBean, m_LogKey.VarName, ctx);
                var hadOrigin = m_Wrapped.PutAndReturnValue(key, value, out var origin);
                GetOrCreateMyLog(ctx).AfterPut(key, origin, hadOrigin, ctx);
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

            public void Commit(TransactionCtx ctx)
            {
                if (IsMapChanged)
                    LogNotify.Notify(m_LogMap.m_LogKey, this, ctx);
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
            
            internal void BeforeRemove(TKey key, TValue value, TransactionCtx ctx)
            {
                Logs.Link(value, null, null!, ctx);
                LogRemove(key, value);
            }
            
            internal void AfterRemove(TKey key, TValue value, TransactionCtx ctx)
            {
                LogRemove(key, value);
                Logs.Link(value, null, null!, ctx);
            }
            
            public void AfterPut(TKey key, TValue? origin, bool hadOrigin, TransactionCtx ctx)
            {
                LogPut(key, origin, hadOrigin);
                if (origin != null)
                    Logs.Link(origin, null, null!, ctx);
            }
        }
    }

    public interface INoteMap : INote
    {
        internal void SetChanged(IList changed, object objRef);
        internal void Merge(INote note);
    }
    
    public class NoteMap<TK, TV> : INoteMap where TK : notnull
    {
        private readonly HashSet<TK> m_Added = new();
        private readonly Dictionary<TK, TV> m_Removed = new();
        private readonly Dictionary<TK, TV> m_Replaced = new();
        private IList? m_Changed;
        private IDictionary? m_ObjRef;
        private Dictionary<TK, TV>? m_ChangedMap;

        public HashSet<TK> Added => m_Added;
        internal Dictionary<TK, TV> Replaced => m_Replaced;
        public Dictionary<TK, TV> Removed => m_Removed;
        protected bool IsMapChanged => m_Added.Count > 0 || m_Removed.Count > 0 || m_Replaced.Count > 0;
        public Dictionary<TK, TV> Changed {
            get
            {
                if (m_ChangedMap != null)
                    return m_ChangedMap;
                
                m_ChangedMap = m_Replaced.Keys.ToDictionary(k => k, k => (TV)m_ObjRef![k]!);
                if (m_Changed == null && m_Added.Count == 0)
                    return m_ChangedMap;
                
                foreach (var k in m_Added)
                    m_ChangedMap[k] = (TV)m_ObjRef![k]!;
                if (m_Changed == null) 
                    return m_ChangedMap;
                
                var set = new HashSet<TV>(new ReferenceEqualityComparer<TV>());
                foreach (var v in m_Changed)
                    set.Add((TV)v);
                foreach (var pair in m_ObjRef!)
                {
                    var entry = (DictionaryEntry)pair;
                    var value = (TV)entry.Value!;
                    if (set.Contains(value))
                        m_ChangedMap[(TK)entry.Key] = value;
                }
                
                return m_ChangedMap;
            }
        }

        public void SetChanged(IList changed, object objRef)
        {
            m_Changed = changed;
            m_ObjRef = (IDictionary)objRef;
        }

        public void Merge(INote note)
        {
            var other = (NoteMap<TK, TV>)note;
            foreach (var k in other.m_Added)
                LogPut(k, default, false);
            foreach (var pair in other.m_Removed)
                LogRemove(pair.Key, pair.Value);
            foreach (var pair in other.m_Replaced)
                LogPut(pair.Key, pair.Value, true);
        }

        protected void LogRemove(TK key, TV value)
        {
            if (m_Added.Remove(key))
                return;
            m_Replaced.Remove(key, out var v);
            m_Removed[key] = v == null ? value : v;
        }

        protected void LogPut(TK key, TV? origin, bool hadOrigin)
        {
            if (m_Added.Contains(key))
                return;
            if (m_Removed.Remove(key, out var v))
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
            return $"added={Strings.ToCustomString(m_Added)} " +
                   $"removed={Strings.ToCustomString(m_Removed)} " +
                   $"replaced={Strings.ToCustomString(m_Replaced)} " +
                   $"changed={Strings.ToCustomStringN(m_Changed)}";
        }
    }
}