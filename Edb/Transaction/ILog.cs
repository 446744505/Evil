namespace Edb
{
    interface ILog
    {
        void Commit();
        void Rollback();
    }
}