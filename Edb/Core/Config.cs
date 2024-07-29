namespace Edb
{
    public class Config
    {
        public bool Verify { get; set; } = true;
        public string DbUrl { get; set; } = "mongodb://localhost:27017";
        public string DbName { get; set; } = "edb_test";
        public EngineType EngineType { get; set; } = EngineType.Mongo;
        /// <summary>
        /// 事务里获取锁的超时时间
        /// </summary>
        public int LockTimeoutMills { get; set; } = 5000;
        public int RetryTimes { get; set; } = 3;
        public int RetryDelay { get; set; } = 200;
        public bool RetrySerial { get; set; } = false;
        public int MarshalPeriod { get; set; } = -1;
        public int CheckpointPeriod { get; set; } = 60000;
        public int MarshalN { get; set; } = 1;
        public int SnapshotFatalTime { get; set; } = 200;
    }
    
    public enum EngineType
    {
        Mongo,
    }
}