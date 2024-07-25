using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace Edb.Test
{
    public class EdbTest
    {
        private void Init()
        {
            var config = new Config();
            config.AddTable(new TableConfig()
            {
                Name = "Player",
                Lock = "Player",
                CacheCapacity = 10,
                IsMemory = false,
            });
            var tables = new List<BaseTable>();
            tables.Add(new TPlayer());
            Edb.I.Start(config, tables);
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
            await Procedure.Submit( async () =>
            {
                await tp.Delete(player.PlayerId);
                Assert.Null(await tp.Select(player.PlayerId));
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var ok = await tp.Insert(player);
                Assert.True(ok);
                var p = await tp.Select(player.PlayerId);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal(player.PlayerName, p.PlayerName);
                return true;
            });
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
            await Procedure.Submit( async () =>
            {
                await tp.Delete(player.PlayerId);
                Assert.Null(await tp.Select(player.PlayerId));
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var ok = await tp.Insert(player);
                Assert.True(ok);
                var up = await tp.Update(player.PlayerId);
                Assert.NotNull(up);
                up.PlayerName = "Bob";
                var p = await tp.Select(player.PlayerId);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal("Bob", p.PlayerName);
                
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

            protected override Player NewValue()
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
            
            public async Task<Player?> Select(long key)
            {
                return await GetAsync(key, false);
            }
            
            public async Task<bool> Insert(Player player)
            {
                return await AddAsync(player.PlayerId, player);
            }
            
            public async Task<bool> Delete(long key)
            {
                return await RemoveAsync(key);
            }
            
            public async Task<Player?> Update(long key)
            {
                return await GetAsync(key, true);
            }
        }
    }
}