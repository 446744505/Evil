namespace Edb
{
    public class XBean
    {
        private static long _objId = 0;

        private long m_ObjId = Interlocked.Increment(ref _objId);
        private XBean? m_Parent;
        private string m_VarName;
        
        internal long ObjId => m_ObjId;
        
        protected XBean(XBean? parent, string varName)
        {
            m_Parent = parent;
            m_VarName = varName;
        }

        internal void Link(XBean? parent, string varName, bool log)
        {
            if (parent != null)
            {
                if (m_Parent != null)
                    throw new XManagedError("ambiguous managed");
                if (ReferenceEquals(parent, this))
                    throw new XManagedError("loop managed");
            }
            else
            {
                if (m_Parent == null)
                    throw new XManagedError("not managed");
            }

            if (log)
            {
                Transaction.CurrentSavepoint.AddIfAbsent(new LogKey(this, "m_Parent"), new LogParent(this));
                m_Parent = parent;
                m_VarName = varName;
            }
        }

        internal virtual void Notify(LogNotify notify)
        {
            m_Parent?.Notify(notify.Push(new LogKey(m_Parent, m_VarName)));
        }

        private TRecord<object,object>? GetRecord()
        {
            var self = this;
            do
            {
                if (self is TRecord<object,object> record)
                    return record;
                self = self.m_Parent;
            } while (self != null);

            return null;
        }
        
        private static readonly Action DoNothing = () => { };

        protected Action VerifyStandaloneOrLockHeld(string methodName, bool readOnly)
        {
            var transaction = Transaction.Current;
            if (transaction == null)
                return DoNothing;
            var record = GetRecord();
            if (record == null)
                return DoNothing;
            switch (transaction.GetLockeyHolderType(record.Lockey))
            {
                case Transaction.LockeyHolderType.Write:
                    return DoNothing;
                case Transaction.LockeyHolderType.Read:
                    if (readOnly)
                        return () => throw new XLockLackedError($"{GetType()}.{methodName}");
                    break;
            }

            throw new XLockLackedError($"{GetType()}.{methodName}");
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        internal class LogParent : ILog
        {
            private readonly XBean m_XBean;
            private readonly XBean? m_Parent;
            private readonly string m_VarName;

            public LogParent(XBean xBean)
            {
                m_XBean = xBean;
                m_Parent = xBean.m_Parent;
                m_VarName = xBean.m_VarName;
            }

            public void Commit()
            {
            }

            public void Rollback()
            {
                m_XBean.m_Parent = m_Parent;
                m_XBean.m_VarName = m_VarName;
            }
        }
    }
}