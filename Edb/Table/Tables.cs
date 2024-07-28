using Evil.Util;

namespace Edb
{
    internal class Tables : IDisposable
    {
        private readonly Dictionary<string, BaseTable> m_Tables = new();
        private readonly LockAsync m_FlushLock = new();
        private readonly List<IStorage> m_Storages = new();
        
        internal LockAsync FlushLock => m_FlushLock;
        internal List<IStorage> Storages => m_Storages;
        internal ILoggerEngine? Logger { get; set; }

        internal void Open(Config config)
        {
            if (Logger != null)
                throw new XError("tables opened");
            switch (config.EngineType)
            {
                case EngineType.Mongo:
                    Logger = new LoggerMongo(config);
                    break;
                default:
                    throw new NotSupportedException($"unknown engine type {config.EngineType}");
            }

            var lockIds = new Dictionary<string, int>();
            var idAlloc = 0;
            foreach (var table in m_Tables.Values)
            {
                var storage = table.Open(config.GetTable(table.Name), Logger);
                if (storage != null)
                    m_Storages.Add(storage);
                
                table.LockId = lockIds.ComputeIfAbsent(table.LockName, _ => ++idAlloc);
            }
        }
        
        internal void Add(BaseTable table)
        {
            if (!m_Tables.TryAdd(table.Name, table))
                throw new ArgumentException($"duplicate table {table.Name}");
        }

        internal TTable<TKey, TValue> Get<TKey, TValue>(string name)
            where TKey : notnull where TValue : class
        {
            if (m_Tables.TryGetValue(name, out var table))
                return (TTable<TKey, TValue>)table;
            throw new ArgumentException($"table {name} not found");
        }

        public void Dispose()
        {
            m_Storages.Clear();
            foreach (var table in m_Tables.Values)
                table.Dispose();
            if (Logger != null)
            {
                Logger.Dispose();
                Logger = null;
            }
        }
    }
}