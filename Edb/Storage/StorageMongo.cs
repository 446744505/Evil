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

        public async Task<bool> InsertAsync(BsonDocument value)
        {
            try
            {
                if (m_Transaction)
                    await m_Collection.InsertOneAsync(m_Logger.Session, value);
                else
                    await m_Collection.InsertOneAsync(value);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task ReplaceAsync(TKey key, BsonDocument value)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                if (m_Transaction)
                    await m_Collection.ReplaceOneAsync(m_Logger.Session, filter, value);
                else
                    await m_Collection.ReplaceOneAsync(filter, value);
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public async Task<BsonDocument?> FindAsync(TKey key)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return await m_Collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public async Task RemoveAsync(TKey key)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                if (m_Transaction)
                    await m_Collection.DeleteOneAsync(m_Logger.Session, filter);
                else
                    await m_Collection.DeleteOneAsync(filter);
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public async Task<bool> ExistsAsync(TKey key)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return await m_Collection.Find(filter).AnyAsync();
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public async Task WalkAsync(Action<BsonDocument> walker)
        {
            try
            {
                var cursor = await m_Collection.FindAsync(FilterDefinition<BsonDocument>.Empty);
                while (await cursor.MoveNextAsync())
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