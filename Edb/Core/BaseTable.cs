namespace Edb
{
    public abstract class BaseTable : ITable, IDisposable
    {
        public virtual string Name { get; }
        public virtual ITable.Persistence PersistenceType { get; }

        public abstract IStorageInterface Open();
        public abstract void Dispose();
    }
}