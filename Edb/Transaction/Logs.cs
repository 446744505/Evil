using System.Collections;

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
        
        public WrapList(LogList<T>? root, IList<T> wrapped)
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
            return new WrapListEnumerator(m_Wrapped);
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

        public int Count => m_Wrapped.Count;
        public bool IsReadOnly => m_Wrapped.IsReadOnly;
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

        private class WrapListEnumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> m_Enumerator;
            public T Current { get; private set; }
            object? IEnumerator.Current => Current;
            
            public WrapListEnumerator(IList<T> wrapped)
            {
                m_Enumerator = wrapped.GetEnumerator();
                Current = default!;
            }
            
            public bool MoveNext()
            {
                if (!m_Enumerator.MoveNext())
                    return false;
                Current = m_Enumerator.Current;
                return true;
            }

            public void Reset()
            {
                m_Enumerator.Reset();
                Current = default!;
            }

            public void Dispose()
            {
                m_Enumerator.Dispose();
            }
        }
    }

    internal class LogList<T> : WrapList<T>
    {
        private readonly LogKey m_LogKey;
        public Action Verify { get; set; } = null!;

        public LogList(LogKey logKey, IList<T> wrapped) : base(null, wrapped)
        {
            m_LogKey = logKey;
        }

        private MyLog GetOrCreateMyLog()
        {
            var sp = Transaction.CurrentSavepoint;
            var log = sp.Get(m_LogKey);
            if (log == null)
            {
                log = new MyLog(this);
                sp.Add(m_LogKey, log);
            }

            return (MyLog)log;
        }

        protected override void BeforeChange()
        {
            Verify.Invoke();
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