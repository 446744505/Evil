using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace Edb.Test
{
    public class CacheTest
    {
        private async Task Init()
        {
            var config = new Config();
            config.AddTable(new TableConfig()
            {
                Name = "Player",
                Lock = "Player",
                CacheCapacity = 1,
                IsMemory = true,
            });
            var tables = new List<BaseTable>();
            tables.Add(new TPlayer());
            await Edb.I.Start(config, tables);
        }
        
        [Fact]
        public async Task Test()
        {
            await Init();

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
            await Procedure.Submit( async () =>
            {
                await tp.Insert(player1);
                Assert.NotNull(await tp.Select(player1.PlayerId));
                await tp.Insert(player2);
                Assert.NotNull(await tp.Select(player2.PlayerId));
                return true;
            });
            await Procedure.Submit( async () =>
            {
                tp.Cache.Clean();
                Assert.Null(await tp.Select(player1.PlayerId));
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