namespace Edb
{
    public abstract class TTableCache<TKey, TValue>
        where TKey : notnull where TValue : class
    {
        private volatile TTable<TKey, TValue> m_Table;
        private volatile RemoveHandler<TKey, TValue> m_RemoveHandler;
        private volatile int m_Capacity;

        internal TTableCache()
        {
        }
        
        internal void Initialize(TTable<TKey, TValue> table, TableConfig config)
        {
            m_Table = table;
            m_Capacity = config.CacheCapacity;
        }
        
        static TTableCache<TKey, TValue> NewInstance(TTable<TKey, TValue> table, TableConfig config)
        {
            var cache = new TTableCacheConcurrentMap<TKey, TValue>();
            cache.Initialize(table, config);
            return cache;
        }
        
        public void SetRemoveHandler(RemoveHandler<TKey, TValue> handler)
        {
            m_RemoveHandler = handler;
        }

        internal void Walk0(ICollection<TRecord<TKey, TValue>> records, Query<TKey, TValue> query)
        {
            foreach (var r in records)
            {
                var lock0 = r.Lockey;
                lock0.RLock();
                try 
                {
                    var value = r.Value;
                    if (value != null)
                    {
                        query.OnQuery(r.Key, value);
                    }
                }
                finally
                {
                    lock0.RUnlock();
                }
            }
        }

        internal void LogAddRemove(TKey key, TRecord<TKey, TValue> r)
        {
            Transaction.CurrentSavepoint.Add(r.CreateLogKey(), new LogAddRemove0(this, key, r));
        }

        internal bool TryRemoveRecord(TRecord<TKey, TValue> r)
        {
            
        }

        public abstract void Clear();
        public abstract void Clean();
        public abstract void Walk(Query<TKey, TValue> query);
        internal abstract ICollection<TRecord<TKey, TValue>> Values();
        internal abstract void Size();
        internal abstract TRecord<TKey, TValue> Get(TKey key);
        internal abstract void AddNoLog(TKey key, TRecord<TKey, TValue> r);
        internal abstract void Add(TKey key, TRecord<TKey, TValue> r);
        internal abstract TRecord<TKey, TValue> Remove(TKey key);

        private class LogAddRemove0 : ILog
        {
            private readonly TTableCache<TKey, TValue> m_Cache;
            private readonly TRecord<TKey, TValue>.State m_State;
            private readonly TKey m_Key;
            private readonly TRecord<TKey, TValue> m_Record;

            public LogAddRemove0(TTableCache<TKey, TValue> cache, TKey key, TRecord<TKey, TValue> r)
            {
                m_Cache = cache;
                m_Key = key;
                m_Record = r;
                m_State = r.Stat;
            }

            public void Commit()
            {
                m_Cache.m_Table.OnRecordChanged(m_Record, true, m_State);
            }

            public void Rollback()
            {
                m_Cache.Remove(m_Key);
            }
        }
    }
    
    public interface Query<TKey, TValue> 
        where TKey : notnull where TValue : class
    {
        public void OnQuery(TKey key, TValue value);
    }
    
    public interface RemoveHandler<TKey, TValue> 
        where TKey : notnull where TValue : class
    {
        public void OnRemoved(TKey key, TValue value);
    }
}