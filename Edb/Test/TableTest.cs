using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace Edb.Test
{
    public class TableTest
    {
        private void Init()
        {
            var tables = new List<BaseTable>();
            tables.Add(new TPlayer());
            Edb.I.Start(new Config(), tables);
        }
        
        [Fact]
        public async Task TestAdd()
        {
            Init();

            var table = Edb.I.Tables.Get<long, Player>("Player");
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            TPlayer tp = (TPlayer)table;
            await Procedure.Submit(() =>
            {
                tp.Delete(player.PlayerId);
                Assert.Null(tp.Select(player.PlayerId));
                return true;
            });
            await Procedure.Submit(() =>
            {
                var ok = tp.Insert(player);
                Assert.True(ok);
                var p = tp.Select(player.PlayerId);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal(player.PlayerName, p.PlayerName);
                return true;
            });
            Edb.I.Dispose();
        }
        
        [Fact]
        public async Task TestUpdate()
        {
            Init();

            var table = Edb.I.Tables.Get<long, Player>("Player");
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            TPlayer tp = (TPlayer)table;
            await Procedure.Submit(() =>
            {
                tp.Delete(player.PlayerId);
                Assert.Null(tp.Select(player.PlayerId));
                return true;
            });
            await Procedure.Submit(() =>
            {
                var ok = tp.Insert(player);
                Assert.True(ok);
                var up = tp.Update(player.PlayerId);
                Assert.NotNull(up);
                up.PlayerName = "Bob";
                var p = tp.Select(player.PlayerId);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal("Bob", p.PlayerName);
                
                return true;
            });
            Edb.I.Dispose();
        }
        
        [Fact]
        public async Task TestCallback()
        {
            Init();

            var table = Edb.I.Tables.Get<long, Player>("Player");
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            TPlayer tp = (TPlayer)table;
            await Procedure.Submit(() =>
            {
                tp.Delete(player.PlayerId);
                Assert.Null(tp.Select(player.PlayerId));
                return true;
            });
            await Procedure.Submit(() =>
            {
                var ok = tp.Insert(player);
                Assert.True(ok);
                var p = tp.Select(player.PlayerId);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal("Alice", p.PlayerName);
                
                return false;
            });
            await Procedure.Submit(() =>
            {
                var p = tp.Select(player.PlayerId);
                Assert.Null(p);
                return true;
            });
            Edb.I.Dispose();
        }
        
        public class Player
        {
            private long playerId;
            private string playerName;
            [BsonId]
            public long PlayerId { get => playerId; set => playerId = value; }

            public string PlayerName { get => playerName; set => playerName = value; }
        }

        public class TPlayer : TTable<long, Player>
        {
            public override string Name => "Player";
            public override TableConfig Config => new()
            {
                Name = "Player",
                Lock = "Player",
                CacheCapacity = 10,
                IsMemory = false,
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
            
            public bool Delete(long key)
            {
                return Remove(key);
            }
            
            public Player? Update(long key)
            {
                return Get(key, true);
            }
        }
    }
}