namespace Edb
{
    internal interface ILog
    {
        internal void Commit(TransactionCtx ctx);
        internal void Rollback();
    }
}