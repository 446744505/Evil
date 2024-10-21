using Evil.Util;
using MongoDB.Bson;

namespace Edb
{
    public abstract partial class TTable<TKey, TValue> : BaseTable
        where TKey : notnull where TValue : class
    {
        private TableConfig m_Config = null!;
        private readonly ListenerMap m_ListenerMap = new();
        private readonly AsyncLocal<LogRecord<TKey, TValue>?> m_LogRecord = new();

        internal TStorage<TKey, TValue>? Storage { get; set; }
        internal TTableCache<TKey, TValue> Cache { get; set; } = null!;
        private LogRecord<TKey, TValue> LogRecord => m_LogRecord.Value ??= new LogRecord<TKey, TValue>(this);
        public bool HasListener => m_ListenerMap.HasListener();
        public override ITable.Persistence PersistenceType => m_Config.IsMemory ? ITable.Persistence.Memory : ITable.Persistence.Db;

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
        
        internal override IStorage? Open(TableConfig config, ILoggerEngine logger)
        {
            if (Storage != null)
                throw new XError($"table {Name} already open");
            m_Config = config;
            LockName = config.Lock ?? Name;
            Cache = TTableCache<TKey, TValue>.NewInstance(this, config);
            Storage = config.IsMemory ? null : new TStorage<TKey, TValue>(this, logger);
            return Storage;
        }

        public Action AddListener(IListener l, string name = "")
        {
            return m_ListenerMap.Add(name, l);
        }

        public override async Task LogNotify(TransactionCtx ctx)
        {
            try
            {
                await LogRecord.LogNotify(m_ListenerMap, ctx);
            } catch (Exception e)
            {
                m_LogRecord.Value = null;
                Log.I.Error(e);
            }
        }
        
        private void OnRecordChanged(TRecord<TKey, TValue> r)
        {
            if (r.Stat == TRecord<TKey, TValue>.State.Remove)
                Cache.Remove(r.Key);
            Storage?.OnRecordChanged(r);
        }

        internal void OnRecordChanged(TRecord<TKey, TValue> r, LogNotify ln, TransactionCtx ctx)
        {
            LogRecord.OnChanged(r, ln, ctx);
            ctx.Current!.AddLastCommitAction(() => OnRecordChanged(r));
        }
        
        internal void OnRecordChanged(TRecord<TKey, TValue> r, bool cc, TRecord<TKey, TValue>.State ss, TransactionCtx ctx)
        {
            LogRecord.OnChanged(r, cc, ss, ctx);
            ctx.Current!.AddLastCommitAction(() => OnRecordChanged(r));
        }

        public override void Dispose()
        {
            Storage = null;
        }

        public abstract TValue? NewValue();
        public abstract TKey MarshalKey(TKey key);
        public abstract BsonDocument MarshalValue(TValue value);
        public abstract TValue UnmarshalValue(BsonDocument value);
    }
}