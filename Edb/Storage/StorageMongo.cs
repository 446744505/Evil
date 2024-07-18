using MongoDB.Bson;
using MongoDB.Driver;

namespace Edb
{
    internal class StorageMongo<TKey> : IStorageEngine<TKey, BsonDocument> where TKey : notnull
    {
        private readonly LoggerMongo m_Logger;
        private readonly string m_TableName;
        private readonly IMongoCollection<BsonDocument> m_Collection;

        public StorageMongo(LoggerMongo logger, string tableName)
        {
            m_Logger = logger;
            m_TableName = tableName;
            m_Collection = logger.Database.GetCollection<BsonDocument>(tableName);
        }

        public async Task<bool> InsertAsync(BsonDocument value)
        {
            try
            {
                await m_Collection.InsertOneAsync(value);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Task ReplaceAsync(TKey key, BsonDocument value)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return m_Collection.ReplaceOneAsync(filter, value);
            }
            catch (Exception e)
            {
                throw new XError(m_TableName, e);
            }
        }

        public async Task<BsonDocument> FindAsync(TKey key)
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

        public Task RemoveAsync(TKey key)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return m_Collection.DeleteOneAsync(filter);
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
            var cursor = await m_Collection.FindAsync(FilterDefinition<BsonDocument>.Empty);
            while (await cursor.MoveNextAsync())
            {
                foreach (var doc in cursor.Current)
                {
                    walker(doc);
                }
            }
        }
    }
}