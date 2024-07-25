
using System.Collections.Concurrent;
using Evil.Util;
using MongoDB.Bson;

namespace Edb
{
    internal class TStorage<TKey, TValue> : IStorageInterface<TKey> 
        where TKey : notnull where TValue : class
    {
        private readonly TTable<TKey, TValue> m_Table;
        private readonly ReaderWriterLockSlim m_SnapshotLock = new();
        private readonly ConcurrentDictionary<TKey, TRecord<TKey, TValue>> m_Changed = new();
        private ConcurrentDictionary<TKey, TRecord<TKey, TValue>> m_Marshal = new();
        private ConcurrentDictionary<TKey, TRecord<TKey, TValue>> m_Snapshot = new();

        #region Metrics

        private long m_CountMarshalN;
        private long m_CountMarshal0;
        private long m_CountSnapshot;
        private long m_CountFlush;
        private long m_CountMarshalNFail;
        
        #endregion

        public IStorageEngine<TKey> Engine { get; }
        
        public TStorage(TTable<TKey, TValue> table, ILoggerEngine logger)
        {
            m_Table = table;
            switch (Edb.I.Config.EngineType)
            {
                case EngineType.Mongo:
                    Engine = new StorageMongo<TKey>((LoggerMongo)logger, table.Name);
                    break;
                default:
                    throw new NotSupportedException($"unknown engine type {Edb.I.Config.EngineType}");
            }
        }

        internal void OnRecordChanged(TRecord<TKey, TValue> r)
        {
            if (r.Stat == TRecord<TKey, TValue>.State.Remove)
            {
                var k = r.Key;
                m_Changed.Remove(k, out _);
                m_Marshal.Remove(k, out _);
            }
            else
            {
                m_Changed[r.Key] = r;
            }
        }

        internal bool IsClean(TKey key)
        {
            return !m_Changed.ContainsKey(key) && !m_Marshal.ContainsKey(key);
        }

        public long MarshalN()
        {
            long tryFail = 0;
            List<TKey> removeKeys = new();
            foreach (var r in m_Changed.Values)
            {
                if (r.TryMarshalN(() => { }))
                {
                    removeKeys.Add(r.Key);
                }
                else
                {
                    tryFail++;
                }
            }
            foreach (var removeKey in removeKeys)
            {
                m_Changed.Remove(removeKey, out _);
            }

            var marshaled = removeKeys.Count;
            m_CountMarshalN += marshaled;
            m_CountMarshalNFail += tryFail;
            
            return marshaled;
        }

        public long Marshal0()
        {
            m_Marshal.PutAll(m_Changed);
            var marshaled = m_Changed.Count;
            foreach (var r in m_Changed.Values)
            {
                r.Marshal0();
            }
            m_Changed.Clear();
            m_CountMarshal0 += marshaled;
            return marshaled;
        }

        public long Snapshot()
        {
            (m_Snapshot, m_Marshal) = (m_Marshal, m_Snapshot);
            var snapshot = m_Snapshot.Count;
            foreach (var r in m_Snapshot.Values)
            {
                r.Snapshot();
            }
            m_CountSnapshot += snapshot;
            return snapshot;
        }

        public async Task<long> Flush0Async()
        {
            var flushed = m_Snapshot.Count;
            foreach (var r in m_Snapshot.Values)
            {
                await r.FlushAsync(this);
            }
            m_CountFlush += flushed;
            return flushed;
        }

        public void Cleanup()
        {
            ConcurrentDictionary<TKey, TRecord<TKey, TValue>> tmp;
            m_SnapshotLock.EnterWriteLock();
            try
            {
                tmp = m_Snapshot;
                m_Snapshot = new();
            }
            finally
            {
                m_SnapshotLock.ExitWriteLock();
            }
            foreach (var r in tmp.Values)
            {
                r.Clear();
            }
        }

        internal async Task<TValue?> Find(TKey key, TTable<TKey, TValue> table)
        {
            BsonDocument? value;
            m_SnapshotLock.EnterReadLock();
            try
            {
                if (m_Snapshot.TryGetValue(key, out var r))
                {
                    value = r.Find();
                    if (value == null)
                        return null;
                }
                else
                {
                    value = null;
                }
            }
            finally
            {
                m_SnapshotLock.ExitReadLock();
            }

            if (value == null)
            {
                var value0 = await Engine.FindAsync(key);
                if (value0 == null)
                    return null;
                value = value0;
            }
            return m_Table.UnmarshalValue(value);
        }

        internal async Task<bool> Exist(TKey key, TTable<TKey, TValue> table)
        {
            m_SnapshotLock.EnterReadLock();
            try
            {
                if (m_Snapshot.TryGetValue(key, out var r))
                {
                    return r.Exist();
                }
            }
            finally
            {
                m_SnapshotLock.ExitReadLock();
            }
            return await Engine.ExistsAsync(key);
        }
    }
}