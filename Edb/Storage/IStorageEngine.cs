using MongoDB.Bson;

namespace Edb
{
    public interface IStorageEngine<TKey> where TKey : notnull
    {
        bool Insert(BsonDocument value);
        void Replace(TKey key, BsonDocument value);
        BsonDocument? Find(TKey key);
        void Remove(TKey key);
        bool Exists(TKey key);
        void Walk(Action<BsonDocument> walker);
    }
}