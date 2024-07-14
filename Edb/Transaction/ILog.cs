namespace Edb
{
    internal interface ILog
    {
        internal void Commit();
        internal void Rollback();
    }
}