using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace Edb.Test
{
    public class MongoStorageTest
    {
        private StorageMongo<long> m_Storage;
        private void Init()
        {
            var logger = new LoggerMongo(new Config());
            m_Storage = new StorageMongo<long>(logger, "player");
        }
        
        [Fact]
        public async void TestCore()
        {
            Init();
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            await m_Storage.RemoveAsync(player.PlayerId);
            Assert.False(await m_Storage.ExistsAsync(player.PlayerId));
            var pb1 = await m_Storage.FindAsync(player.PlayerId);
            Assert.Null(pb1);
            var doc = player.ToBsonDocument();
            var ok1 = await m_Storage.InsertAsync(doc);
            Assert.True(ok1);
            Assert.True(await m_Storage.ExistsAsync(player.PlayerId));
            var ok2 = await m_Storage.InsertAsync(doc);
            Assert.False(ok2);
            var pb2 = await m_Storage.FindAsync(player.PlayerId);
            var p = BsonSerializer.Deserialize<Player>(pb2);
            Assert.Equal(player.PlayerId, p.PlayerId);
            Assert.Equal(player.PlayerName, p.PlayerName);
        }

        [Fact]
        public async void TestWalk()
        {
            Init();
            var player1 = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            var player2 = new Player()
            {
                PlayerId = 2,
                PlayerName = "Bob"
            };
            await m_Storage.RemoveAsync(player1.PlayerId);
            await m_Storage.RemoveAsync(player2.PlayerId);
            var doc1 = player1.ToBsonDocument();
            await m_Storage.InsertAsync(doc1);
            var doc2 = player2.ToBsonDocument();
            await m_Storage.InsertAsync(doc2);
            List<Player> players = new();
            await m_Storage.WalkAsync((doc) =>
            {
                var player = BsonSerializer.Deserialize<Player>(doc);
                players.Add(player);
            });
            Assert.Equal(2, players.Count);
        }
    }

    public class Player
    {
        [BsonId]
        public long PlayerId { get; set; }
        public string PlayerName { get; set; }
    }
}