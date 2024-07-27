using MongoDB.Driver;

namespace Edb
{
    internal class LoggerMongo : ILoggerEngine
    {
        private readonly IMongoDatabase m_Database;
        public IMongoDatabase Database => m_Database;
        
        public LoggerMongo(Config config)
        {
            var client = new MongoClient(config.DbUrl);
            m_Database = client.GetDatabase(config.DbName);
        }
        
        public void Dispose()
        {
        }

        public void Checkpoint()
        {
            
        }

        public void Backup(string path, bool increment)
        {
        }

        public void DropTables(string[] tableNames)
        {
            foreach (var tableName in tableNames)
            {
                m_Database.DropCollection(tableName);
            }
        }
    }
}