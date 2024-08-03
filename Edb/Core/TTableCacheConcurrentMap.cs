using System.Collections.Concurrent;

namespace Edb
{
    public class TTableCacheConcurrentMap<TKey, TValue> : TTableCache<TKey, TValue>
        where TKey : notnull where TValue : class
    {
        private readonly ConcurrentDictionary<TKey, TRecord<TKey, TValue>> m_Cache = new();
        private Func<Task> m_CleanWorker = null!;
        private bool m_Cleaning;

        internal override int Count => m_Cache.Count;

        internal override void Initialize(TTable<TKey, TValue> table, TableConfig config)
        {
            base.Initialize(table, config);
            m_CleanWorker = async () =>
            {
                if (SetCleaning())
                {
                    await CleanNow();
                    ResetCleaning();
                }
            };
            var delay = 3600 * 1000;
            var initDelay = Edb.I.Random.Next(delay);
            Edb.I.Executor.Tick(m_CleanWorker, initDelay, delay);
        }

        private bool SetCleaning()
        {
            lock (this)
            {
                if (m_Cleaning)
                {
                    return false;
                }

                m_Cleaning = true;
                return true;
            }
        }
        
        private void ResetCleaning()
        {
            lock (this)
            {
                m_Cleaning = false;
            }
        }

        private async Task CleanNow()
        {
            if (m_Capacity <= 0)
                return;
            if (Count <= m_Capacity)
                return;
            var sorted = new PriorityQueue<AccessTimeRecord<TKey, TValue>, long>();
            foreach (var r in m_Cache.Values)
            {
                sorted.Enqueue(new AccessTimeRecord<TKey, TValue>(r), r.LastAccessTime);
            }

            for (var clenaN = Count - m_Capacity + 255; clenaN > 0;)
            {
                if (!sorted.TryDequeue(out var ar, out _))
                {
                    break;
                }
                if (ar.m_AccessTime != ar.m_Record.LastAccessTime)
                    continue;
                var removed = await TryRemoveRecord(ar.m_Record);
                if (removed)
                    clenaN--;
            }
        }

        public override void Clear()
        {
            if (m_Table.PersistenceType != ITable.Persistence.Memory)
                throw new NotSupportedException();
            m_Cache.Clear();
        }

        public override void Clean()
        {
            m_CleanWorker();
        }

        public override void Walk(Query<TKey, TValue> query)
        {
            Walk0(m_Cache.Values, query);
        }

        internal override ICollection<TRecord<TKey, TValue>> Values()
        {
            return m_Cache.Values;
        }

        internal override TRecord<TKey, TValue>? Get(TKey key)
        {
            return m_Cache.TryGetValue(key, out var r) ? r.Access() : null;
        }

        internal override void AddNoLog(TKey key, TRecord<TKey, TValue> r)
        {
            if (!m_Cache.TryAdd(key, r))
                throw new XError("cache.AddNoLog duplicate record");
        }

        internal override void Add(TKey key, TRecord<TKey, TValue> r)
        {
            if (!m_Cache.TryAdd(key, r))
                throw new XError("cache.Add duplicate record");
            LogAddRemove(key, r);
        }

        internal override TRecord<TKey, TValue>? Remove(TKey key)
        {
            m_Cache.Remove(key, out var r);
            return r;
        }
        
        private struct AccessTimeRecord<TK, TV> : IComparable<AccessTimeRecord<TK, TV>>
            where TK : notnull where TV : class
        {
            internal readonly long m_AccessTime;
            internal readonly TRecord<TK, TV> m_Record;

            public AccessTimeRecord(TRecord<TK, TV> record)
            {
                m_Record = record;
                m_AccessTime = record.LastAccessTime;
            }
            
            public int CompareTo(AccessTimeRecord<TK, TV> other)
            {
                return m_AccessTime < other.m_AccessTime ? -1 : m_AccessTime > other.m_AccessTime ? 1 : 0;
            }
        }
    }
}