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
        public void TestCore()
        {
            Init();
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            m_Storage.Remove(player.PlayerId);
            Assert.False(m_Storage.Exists(player.PlayerId));
            var pb1 = m_Storage.Find(player.PlayerId);
            Assert.Null(pb1);
            var doc = player.ToBsonDocument();
            var ok1 = m_Storage.Insert(doc);
            Assert.True(ok1);
            Assert.True(m_Storage.Exists(player.PlayerId));
            var ok2 = m_Storage.Insert(doc);
            Assert.False(ok2);
            var pb2 = m_Storage.Find(player.PlayerId);
            var p = BsonSerializer.Deserialize<Player>((BsonDocument)pb2!);
            Assert.Equal(player.PlayerId, p.PlayerId);
            Assert.Equal(player.PlayerName, p.PlayerName);
        }

        [Fact]
        public void TestWalk()
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
            m_Storage.Remove(player1.PlayerId);
            m_Storage.Remove(player2.PlayerId);
            var doc1 = player1.ToBsonDocument();
            m_Storage.Insert(doc1);
            var doc2 = player2.ToBsonDocument();
            m_Storage.Insert(doc2);
            List<Player> players = new();
            m_Storage.Walk((doc) =>
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