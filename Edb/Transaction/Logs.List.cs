using System.Collections;

namespace Edb
{
    public partial class Logs
    {
        public static IList<T> LogList<T>(XBean xBean, string varName, Action verify)
        {
            var key = new LogKey(xBean, varName);
            var wrappers = Transaction.Current!.Wrappers;
            if (!wrappers.TryGetValue(key, out var log))
            {
                wrappers[key] = log = new LogList<T>(key, (key.Value as List<T>)!);
            }

            return ((LogList<T>)log).SetVerify(verify);
        }
    }
    
    internal class WrapList<T> : IList<T>
    {
        private readonly LogList<T>? m_Root;
        private readonly List<T> m_Wrapped; // 直接用List方便后续扩展
        
        protected List<T> Wrapped => m_Wrapped;
        public int Count => m_Wrapped.Count;
        public bool IsReadOnly => false;
        
        protected WrapList(LogList<T>? root, List<T> wrapped)
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

        public LogList(LogKey logKey, List<T> wrapped) : base(null, wrapped)
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
            m_Verify();
            GetOrCreateMyLog().BeforeChange();
        }
        
        protected override void AfterAdd(T item)
        {
            Logs.Link(item, m_LogKey.XBean, m_LogKey.VarName);
        }
        
        protected override void BeforeRemove(T item)
        {
            Logs.Link(item, null, null!);
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
}