namespace Edb
{
    public abstract class BaseTable : ITable, IDisposable
    {
        protected int m_LockId;
        public string LockName { get; set; }
        public int LockId
        {
            get => m_LockId; 
            set => m_LockId = value;
        }
        
        public virtual string Name { get; }
        public virtual ITable.Persistence PersistenceType { get; }
        public abstract void LogNofify();
        internal abstract IStorage? Open(TableConfig config, ILoggerEngine logger);
        public abstract void Dispose();
    }
}