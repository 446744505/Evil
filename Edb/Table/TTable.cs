namespace Edb
{
    public abstract class TTable<TKey, TValue> : BaseTable<TKey> 
        where TKey : notnull where TValue : class
    {
        private int m_LockId;
        private TableConfig m_Config;
        
        public string LockName { get; set; }

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
    }
}