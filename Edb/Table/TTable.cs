namespace Edb
{
    public abstract class TTable<TKey, TValue> : BaseTable<TKey> 
        where TKey : notnull where TValue : class
    {
        private int m_LockId;
        private TableConfig m_Config;
        
        public string LockName { get; set; }

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