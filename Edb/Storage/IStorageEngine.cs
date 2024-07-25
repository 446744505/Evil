using MongoDB.Bson;

namespace Edb
{
    public interface IStorageEngine<TKey> where TKey : notnull
    {
        Task<bool> InsertAsync(BsonDocument value);
        Task ReplaceAsync(TKey key, BsonDocument value);
        Task<BsonDocument?> FindAsync(TKey key);
        Task RemoveAsync(TKey key);
        Task<bool> ExistsAsync(TKey key);
        Task WalkAsync(Action<BsonDocument> walker);
    }
}