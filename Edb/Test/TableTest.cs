using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace Edb.Test
{
    public class TableTest
    {
        private async Task Init()
        {
            var tables = new List<BaseTable>();
            tables.Add(new TPlayer());
            await Edb.I.Start(new Config(), tables);
        }
        
        [Fact]
        public async Task TestAdd()
        {
            await Init();

            var table = Edb.I.Tables.Get<long, Player>("Player");
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            TPlayer tp = (TPlayer)table;
            await Procedure.Submit( async ctx =>
            {
                await tp.Delete(player.PlayerId, ctx);
                Assert.Null(await tp.Select(player.PlayerId, ctx));
                return true;
            });
            await Procedure.Submit(async ctx =>
            {
                var ok = await tp.Insert(player, ctx);
                Assert.True(ok);
                var p = await tp.Select(player.PlayerId, ctx);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal(player.PlayerName, p.PlayerName);
                return true;
            });
            await Edb.I.DisposeAsync();
        }
        
        [Fact]
        public async Task TestUpdate()
        {
            await Init();

            var table = Edb.I.Tables.Get<long, Player>("Player");
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            TPlayer tp = (TPlayer)table;
            await Procedure.Submit( async ctx =>
            {
                await tp.Delete(player.PlayerId, ctx);
                Assert.Null(await tp.Select(player.PlayerId, ctx));
                return true;
            });
            await Procedure.Submit(async ctx =>
            {
                var ok = await tp.Insert(player, ctx);
                Assert.True(ok);
                var up = await tp.Update(player.PlayerId, ctx);
                Assert.NotNull(up);
                up.PlayerName = "Bob";
                var p = await tp.Select(player.PlayerId, ctx);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal("Bob", p.PlayerName);
                
                return true;
            });
            await Edb.I.DisposeAsync();
        }
        
        [Fact]
        public async Task TestCallback()
        {
            await Init();

            var table = Edb.I.Tables.Get<long, Player>("Player");
            var player = new Player()
            {
                PlayerId = 1,
                PlayerName = "Alice"
            };
            TPlayer tp = (TPlayer)table;
            await Procedure.Submit( async ctx =>
            {
                await tp.Delete(player.PlayerId, ctx);
                Assert.Null(await tp.Select(player.PlayerId, ctx));
                return true;
            });
            await Procedure.Submit(async ctx =>
            {
                var ok = await tp.Insert(player, ctx);
                Assert.True(ok);
                var p = await tp.Select(player.PlayerId, ctx);
                Assert.NotNull(p);
                Assert.Equal(player.PlayerId, p.PlayerId);
                Assert.Equal("Alice", p.PlayerName);
                
                return false;
            });
            await Procedure.Submit(async ctx =>
            {
                var p = await tp.Select(player.PlayerId, ctx);
                Assert.Null(p);
                return true;
            });
            await Edb.I.DisposeAsync();
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
            
            public async Task<Player?> Select(long key, TransactionCtx ctx)
            {
                return await GetAsync(key, false, ctx);
            }
            
            public async Task<bool> Insert(Player player, TransactionCtx ctx)
            {
                return await AddAsync(player.PlayerId, player, ctx);
            }
            
            public async Task<bool> Delete(long key, TransactionCtx ctx)
            {
                return await RemoveAsync(key, ctx);
            }
            
            public async Task<Player?> Update(long key, TransactionCtx ctx)
            {
                return await GetAsync(key, true, ctx);
            }
        }
    }
}