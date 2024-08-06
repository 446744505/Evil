using MongoDB.Bson;

namespace Edb
{
    public interface IMongoCodec
    {
        void Unmarshal(BsonDocument doc);
    }
}