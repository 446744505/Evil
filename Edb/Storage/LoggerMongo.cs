using MongoDB.Driver;

namespace Edb
{
    internal class LoggerMongo : ILoggerEngine
    {
        private readonly bool m_Transaction;
        private readonly IMongoDatabase m_Database;
        private readonly MongoClient m_Client;
        public IMongoDatabase Database => m_Database;
        public bool Transaction => m_Transaction;
        public IClientSessionHandle? Session { get; private set; }
        
        public LoggerMongo(Config config)
        {
            m_Transaction = config.DbTransaction;
            m_Client = new MongoClient(config.DbUrl);
            m_Database = m_Client.GetDatabase(config.DbName);
        }
        
        public void Dispose()
        {
        }

        public void BeforeFlush()
        {
            if (!m_Transaction)
                return;
            
            Session = m_Client.StartSession();
            Session.StartTransaction();
        }
        
        public void AfterFlush(bool success)
        {
            if (!m_Transaction)
                return;
            
            if (success)
                Session!.CommitTransaction();
            else
                Session!.AbortTransaction();
            
            Session.Dispose();
            Session = null;
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