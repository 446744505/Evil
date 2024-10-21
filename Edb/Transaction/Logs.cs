
namespace Edb
{
    public sealed partial class Logs
    {
        private Logs() {}

        public static void LogObject(XBean xBean, string varName, TransactionCtx ctx)
        {
            var key = new LogKey(xBean, varName);
            var sp = ctx.Current!.CurrentSavepoint;
            if (sp.Get(key) == null)
                sp.Add(key, new LogObject(key));
        }

        internal static void Link(object? bean, XBean? parent, string varName, TransactionCtx ctx, bool log = true)
        {
            switch (bean)
            {
                case null:
                    throw new NullReferenceException();
                case XBean xBean:
                    xBean.Link(parent, varName, log, ctx);
                    break;
            }
        }
    }

    internal struct LogObject : INote, ILog
    {
        private readonly LogKey m_LogKey;
        private readonly object? m_Origin;
        
        internal LogObject(LogKey logKey)
        {
            m_LogKey = logKey;
            m_Origin = m_LogKey.Value;
        }
        
        public void Commit(TransactionCtx ctx)
        {
            LogNotify.Notify(m_LogKey, this, ctx);
        }

        public void Rollback()
        {
            m_LogKey.Value = m_Origin;
        }

        public override string? ToString()
        {
            return m_Origin?.ToString();
        }
    }
}