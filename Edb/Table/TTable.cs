namespace Edb
{
    public abstract class TTable<TKey, TValue> : BaseTable
        where TKey : notnull where TValue : class
    {
        private int m_LockId;
        private TableConfig m_Config;
        
        public string LockName { get; set; }
        internal TStorage<TKey, TValue>? Storage { get; set; }
        internal TTableCache<TKey, TValue> Cache { get; set; }

        #region Metrics

        private long m_CountAdd;
        private long m_CountAddMiss;
        private long m_CountAddStorageMiss;
        
        private long m_CountGet;
        private long m_CountGetMiss;
        private long m_CountGetStorageMiss;
        
        private long m_CountRemove;
        private long m_CountRemoveMiss;
        private long m_CountRemoveStorageMiss;

        #endregion

        protected TTable()
        {
        }
        
        internal IStorageInterface<TKey>? Open(TableConfig config, ILoggerEngine logger)
        {
            if (Storage != null)
                throw new XError($"table {Name} already open");
            m_Config = config;
            LockName = config.Lock ?? Name;
            Cache = TTableCache<TKey, TValue>.NewInstance(this, config);
            Storage = config.IsMemory ? null : new TStorage<TKey, TValue>(this, logger);
            return Storage;
        }

        public override void LogNofify()
        {
            
        }

        internal void OnRecordChanged(TRecord<TKey, TValue> r, LogNotify ln)
        {
            
        }
        
        internal void OnRecordChanged(TRecord<TKey, TValue> r, bool cc, TRecord<TKey, TValue>.State ss)
        {
            
        }
        
        public abstract TKey MarshalKey(TKey key);
        public abstract object MarshalValue(TValue value);
        public abstract TValue UnmarshalValue(object value);
    }
}