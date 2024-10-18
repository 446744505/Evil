namespace Edb
{
    public abstract class BaseTable : ITable, IDisposable
    {
        protected int m_LockId;
        public string LockName { get; set; } = null!;
        public int LockId
        {
            get => m_LockId; 
            set => m_LockId = value;
        }
        
        public virtual string Name => null!;
        public virtual TableConfig Config { get; set; } = null!;
        public virtual ITable.Persistence PersistenceType => ITable.Persistence.Db;
        public abstract void LogNotify();
        internal abstract IStorage? Open(TableConfig config, ILoggerEngine logger);
        public abstract void Dispose();
    }
}