namespace Edb
{
    public abstract class BaseTable : ITable, IDisposable
    {
        public virtual string Name { get; }
        public virtual ITable.Persistence PersistenceType { get; }
        public abstract void LogNofify();
        public abstract IStorageInterface<TKey> Open<TKey>(TableConfig config, ILoggerEngine logger);
        public abstract void Dispose();
    }
}