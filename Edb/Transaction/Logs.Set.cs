using System.Collections;

namespace Edb
{
    public partial class Logs
    {
        public static ISet<T> LogSet<T>(XBean xBean, string varName, Action verify)
        {
            var key = new LogKey(xBean, varName);
            var wrappers = Transaction.Current!.Wrappers;
            if (!wrappers.TryGetValue(key, out var log))
            {
                wrappers[key] = log = new LogSet<T>(key, (key.Value as HashSet<T>)!);
            }
         
            return ((LogSet<T>)log).SetVerify(verify);
        }
    }
    
    internal class LogSet<T> : ISet<T>
    {
        private readonly LogKey m_LogKey;
        private readonly HashSet<T> m_Wrapped; // 直接用HashSet方便后续扩展
        private Action m_Verify = null!;

        public int Count => m_Wrapped.Count;
        public bool IsReadOnly => false;
        
        public LogSet(LogKey logKey, HashSet<T> wrapped)
        {
            m_LogKey = logKey;
            m_Wrapped = wrapped;
        }

        public LogSet<T> SetVerify(Action verify)
        {
            m_Verify = verify;
            return this;
        }

        private MyLog<T> GetOrCreateMyLog()
        {
            var sp = Transaction.CurrentSavepoint;
            var log = sp.Get(m_LogKey);
            if (log != null) 
                return (MyLog<T>)log;
            
            log = new MyLog<T>(this);
            sp.Add(m_LogKey, log);

            return (MyLog<T>)log;
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
            m_Verify();
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
            m_Verify();
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
            m_Verify();
            if (!m_Wrapped.Remove(item)) 
                return false;
            
            GetOrCreateMyLog().AfterRemove(item);
            return true;

        }
        
        public override int GetHashCode()
        {
            return m_Wrapped.GetHashCode();
        }

        private class MyLog<T> : NoteSet<T>, ILog
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
    }
    
    public class NoteSet<T> : INote
    {
        private readonly HashSet<T> m_Added = new();
        private readonly HashSet<T> m_Removed = new();
        private readonly HashSet<T> m_Eldest = new();
            
        protected ISet<T> Added => m_Added;
        protected ISet<T> Removed => m_Removed;
        protected ISet<T> Eldest => m_Eldest;
        protected bool IsSetChanged => m_Added.Count > 0 || m_Removed.Count > 0;

        internal void Merge(INote note)
        {
            var other = (NoteSet<T>)note;
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