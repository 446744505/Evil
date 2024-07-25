namespace Edb
{
    public class Config
    {
        private readonly Dictionary<string, TableConfig> m_TableConfigs = new();
        public bool Verify { get; set; } = true;
        public string DbUrl { get; set; } = "mongodb://localhost:27017";
        public string DbName { get; set; } = "edb_test";
        public EngineType EngineType { get; set; } = EngineType.Mongo;
        /// <summary>
        /// 事务里获取锁的超时时间
        /// </summary>
        public int LockTimeoutMills { get; set; } = 5000;
        public int RetryTimes { get; set; } = 3;
        public int RetryDelay { get; set; } = 100;
        public bool RetrySerial { get; set; } = false;
        
        public TableConfig? GetTable(string name)
        {
            m_TableConfigs.TryGetValue(name, out var config);
            return config;
        }

        public void AddTable(TableConfig config)
        {
            m_TableConfigs[config.Name] = config;
        }
    }
    
    public enum EngineType
    {
        Mongo,
    }
}