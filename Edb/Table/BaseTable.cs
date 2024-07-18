namespace Edb
{
    public abstract class BaseTable<TKey> : ITable, IDisposable where TKey : notnull
    {
        public virtual string Name { get; }
        public virtual ITable.Persistence PersistenceType { get; }

        public abstract IStorageInterface<TKey> Open<TKey>();
        public abstract void Dispose();
    }
}