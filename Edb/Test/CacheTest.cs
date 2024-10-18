using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace Edb.Test
{
    public class CacheTest
    {
        private void Init()
        {
            var tables = new List<BaseTable>();
            tables.Add(new TPlayer());
            Edb.I.Start(new Config(), tables);
        }
        
        [Fact]
        public async Task Test()
        {
            Init();

            var table = Edb.I.Tables.Get<long, Player>("Player");
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
            TPlayer tp = (TPlayer)table;
            await Procedure.Submit( () =>
            {
                tp.Insert(player1);
                Assert.NotNull(tp.Select(player1.PlayerId));
                tp.Insert(player2);
                Assert.NotNull(tp.Select(player2.PlayerId));
                return true;
            });
            await Procedure.Submit( () =>
            {
                tp.Cache.Clean();
                Assert.Null(tp.Select(player1.PlayerId));
                return true;
            });
        }
        
        public class Player
        {
            [BsonId]
            public long PlayerId { get; set; }
            public string PlayerName { get; set; }
        }

        public class TPlayer : TTable<long, Player>
        {
            public override string Name => "Player";
            public override TableConfig Config => new()
            {
                Name = "Player",
                Lock = "Player",
                CacheCapacity = 1,
                IsMemory = true,
            };

            public override Player NewValue()
            {
                return new Player();
            }

            public override long MarshalKey(long key)
            {
                return key;
            }

            public override BsonDocument MarshalValue(Player value)
            {
                return value.ToBsonDocument();
            }

            public override Player UnmarshalValue(BsonDocument value)
            {
                return BsonSerializer.Deserialize<Player>(value);
            }
            
            public Player? Select(long key)
            {
                return Get(key, false);
            }
            
            public bool Insert(Player player)
            {
                return Add(player.PlayerId, player);
            }
            
            public async Task<bool> Delete(long key)
            {
                return Remove(key);
            }
            
            public Player Update(long key)
            {
                return Get(key, true);
            }
        }
    }
}