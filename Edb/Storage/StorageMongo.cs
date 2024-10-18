using MongoDB.Bson;
using MongoDB.Driver;

namespace Edb
{
    internal class StorageMongo<TKey> : IStorageEngine<TKey> where TKey : notnull
    {
        private readonly bool m_Transaction;
        private readonly LoggerMongo m_Logger;
        private readonly string m_TableName;
        private readonly IMongoCollection<BsonDocument> m_Collection;

        public StorageMongo(LoggerMongo logger, string tableName)
        {
            m_Transaction = logger.Transaction;
            m_Logger = logger;
            m_TableName = tableName;
            m_Collection = logger.Database.GetCollection<BsonDocument>(tableName);
        }

        public bool Insert(BsonDocument value)
        {
            try
            {
                if (m_Transaction)
                    m_Collection.InsertOne(m_Logger.Session, value);
                else
                    m_Collection.InsertOne(value);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void Replace(TKey key, BsonDocument value)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                if (m_Transaction)
                    m_Collection.ReplaceOne(m_Logger.Session, filter, value);
                else
                    m_Collection.ReplaceOne(filter, value);
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public BsonDocument? Find(TKey key)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return m_Collection.Find(filter).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public void Remove(TKey key)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                if (m_Transaction)
                    m_Collection.DeleteOne(m_Logger.Session, filter);
                else
                    m_Collection.DeleteOne(filter);
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public bool Exists(TKey key)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return m_Collection.Find(filter).Any();
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public void Walk(Action<BsonDocument> walker)
        {
            try
            {
                var cursor = m_Collection.FindAsync(FilterDefinition<BsonDocument>.Empty).Result;
                while (cursor.MoveNext())
                {
                    foreach (var doc in cursor.Current)
                    {
                        walker(doc);
                    }
                }
            } catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }
    }
}