namespace Edb
{
    public abstract class BaseTable<TKey, TValue> : ITable, IDisposable where TKey : notnull
    {
        public virtual string Name { get; }
        public virtual ITable.Persistence PersistenceType { get; }

        public abstract IStorageInterface<TKey, TValue> Open<TKey, TValue>();
        public abstract void Dispose();
    }
}