using System.Collections;

namespace Edb
{
    public partial class Logs
    {
        public static IList<T> LogList<T>(XBean xBean, string varName, Action verify, TransactionCtx ctx)
        {
            var key = new LogKey(xBean, varName);
            var wrappers = ctx.Current!.Wrappers;
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

        protected virtual void BeforeChange(TransactionCtx ctx)
        {
            m_Root!.BeforeChange(ctx);
        }

        protected virtual void AfterAdd(T item, TransactionCtx ctx)
        {
            m_Root!.AfterAdd(item, ctx);
        }
        
        protected virtual void BeforeRemove(T item, TransactionCtx ctx)
        {
            m_Root!.BeforeRemove(item, ctx);
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

        public void Add(T item, TransactionCtx ctx)
        {
            BeforeChange(ctx);
            m_Wrapped.Add(item);
            AfterAdd(item, ctx);
        }

        public void Clear(TransactionCtx ctx)
        {
            BeforeChange(ctx);
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

        public bool Remove(T item, TransactionCtx ctx)
        {
            var idx = IndexOf(item);
            if (idx < 0)
                return false;
            RemoveAt(idx, ctx);
            return true;
        }
        
        public int IndexOf(T item)
        {
            return m_Wrapped.IndexOf(item);
        }

        public void Insert(int index, T item, TransactionCtx ctx)
        {
            BeforeChange(ctx);
            m_Wrapped.Insert(index, item);
            AfterAdd(item, ctx);
        }

        public void RemoveAt(int index, TransactionCtx ctx)
        {
            BeforeChange(ctx);
            BeforeRemove(m_Wrapped[index], ctx);
            m_Wrapped.RemoveAt(index);
        }

        public T this[int index]
        {
            get => m_Wrapped[index];
            set
            {
                BeforeChange(ctx);
                BeforeRemove(m_Wrapped[index], ctx);
                m_Wrapped[index] = value;
                AfterAdd(value, ctx);
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

        protected override void BeforeChange(TransactionCtx ctx)
        {
            m_Verify();
            GetOrCreateMyLog(ctx).BeforeChange();
        }
        
        protected override void AfterAdd(T item, TransactionCtx ctx)
        {
            Logs.Link(item, m_LogKey.XBean, m_LogKey.VarName, ctx);
        }
        
        protected override void BeforeRemove(T item, TransactionCtx ctx)
        {
            Logs.Link(item, null, null!, ctx);
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

            public void Commit(TransactionCtx ctx)
            {
                if (m_SavedOnWrite != null)
                    LogNotify.Notify(m_LogList.m_LogKey, this, ctx);
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