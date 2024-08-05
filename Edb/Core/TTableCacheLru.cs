using Evil.Util;

namespace Edb
{
    public class TTableCacheLru<TKey, TValue> : TTableCache<TKey, TValue>
        where TKey : notnull where TValue : class
    {
        private LockAsync m_Lock = new();
        private LruCache<TKey, TRecord<TKey, TValue>> m_Cache;
        private Cleaner m_Cleaner;

        internal override int Count
        {
            get
            {
                var release = m_Lock.WLock();
                try
                {
                    return m_Cache.Count;
                }
                finally
                {
                    m_Lock.WUnlock(release);
                }
            }
        }

        internal override void Initialize(TTable<TKey, TValue> table, TableConfig config)
        {
            base.Initialize(table, config);
            m_Cleaner = new Cleaner(this);
            m_Cache = new LruCache<TKey, TRecord<TKey, TValue>>(config.CacheCapacity, () =>
            {
                m_Cleaner.Start();
            });
        }
        

        public override async Task Clear()
        {
            if (m_Table.PersistenceType != ITable.Persistence.Memory)
                throw new NotSupportedException();
            var release = await m_Lock.WLockAsync();
            try
            {
                m_Cache.Clear();
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        public override void Clean()
        {
        }

        public override async Task Walk(Query<TKey, TValue> query)
        {
            List<TRecord<TKey, TValue>> records = new(); 
            var release = await m_Lock.WLockAsync();
            try
            {
                foreach (var pair in m_Cache)
                {
                    records.Add(pair.Value);
                }
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
            await Walk0(records, query);
        }

        internal override ICollection<TRecord<TKey, TValue>> Values()
        {
            var release = m_Lock.WLock();
            try
            {
                return m_Cache.Values;
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        internal override TRecord<TKey, TValue>? Get(TKey key)
        {
            var release = m_Lock.WLock();
            try
            {
                return m_Cache.Lookup(key);
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        internal override void AddNoLog(TKey key, TRecord<TKey, TValue> r)
        {
            var release = m_Lock.WLock();
            try
            {
                if (m_Cache.Contains(key))
                    throw new XError("cache.Add duplicate record");
                m_Cache.Add(key, r);
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        internal override void Add(TKey key, TRecord<TKey, TValue> r)
        {
            var release = m_Lock.WLock();
            try
            {
                if (m_Cache.Contains(key))
                    throw new XError("cache.Add duplicate record");
                LogAddRemove(key, r);
                m_Cache.Add(key, r);
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        internal override bool Remove(TKey key)
        {
            var release = m_Lock.WLock();
            try
            {
                return m_Cache.Remove(key);
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        private class Cleaner
        {
            private int m_Running;
            private readonly TTableCacheLru<TKey, TValue> m_Lru;

            public Cleaner(TTableCacheLru<TKey, TValue> lru)
            {
                m_Lru = lru;
            }

            public void Start()
            {
                if (0 == Interlocked.CompareExchange(ref m_Running, 1, 0))
                {
                    Edb.I.Executor.ExecuteAsync(Run);
                }
            }

            private async Task Run()
            {
                var flushRelease = await Edb.I.Tables.FlushLock.RLockAsync();
                try
                {
                    var eldest = new List<TRecord<TKey, TValue>>();
                    var release = await m_Lru.m_Lock.WLockAsync();
                    try
                    {
                        var capacity = m_Lru.m_Capacity;
                        var count = m_Lru.m_Cache.Count;
                        if (count <= capacity)
                        {
                            return;
                        }

                        var cleanN = Math.Min(count - capacity + 255, count);
                        foreach (var pair in m_Lru.m_Cache)
                        {
                            if (cleanN <= 0)
                                break;

                            eldest.Add(pair.Value);
                            cleanN--;
                        }
                    }
                    finally
                    {
                        m_Lru.m_Lock.WUnlock(release);
                    }
                    foreach (var record in eldest)
                    {
                        await m_Lru.TryRemoveRecord(record);
                    }
                }
                finally
                {
                    Edb.I.Tables.FlushLock.RUnlock(flushRelease);
                    Interlocked.Exchange(ref m_Running, 0);
                }
            }
        }
    }
}