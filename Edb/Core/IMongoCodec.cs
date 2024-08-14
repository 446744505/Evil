using MongoDB.Bson;

namespace Edb
{
    public interface IMongoCodec<T>
    {
        T Unmarshal(BsonDocument doc);
        BsonDocument Marshal(BsonDocument doc);
    }
}