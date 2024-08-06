using System.Threading.Tasks;
using Edb;
using Evil.Event;
using Evil.Util;
using XBean;
using Xunit;
using IListener = Edb.IListener;

namespace Game.Test
{
    public class NotifyTest
    {
        private async Task Init()
        {
            await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
            await Procedure.Submit(async () =>
            {
                await XTable.Player.Delete(1);
                
                var p = new Player()
                {
                    PlayerId = 1,
                    Level = 1,
                    PlayerName = "player1",
                };
                Assert.True(await XTable.Player.Insert(p));

                await XTable.PlayerHero.Delete(1);
                var skill = new XBean.HeroSkill()
                {
                    CfgId = 1,
                    Level = 1,
                };
                var hero = new XBean.Hero()
                {
                    HeroId = 1,
                    Star = 1,
                };
                hero.Skills.Add(skill);
                var ph = new PlayerHero()
                {
                    PlayerId = 1,
                };
                ph.Heroes[hero.HeroId] = hero;
                Assert.True(await XTable.PlayerHero.Insert(ph));

                return true;
            });
        }
        
        [Fact]
        public async Task TestInsert()
        {
            await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
            XTable.Tables.Player.AddListener(new PlayerListenerLevel(), "level");
            await Procedure.Submit(async () =>
            {
                await XTable.Player.Delete(1);
                var p = new Player()
                {
                    PlayerId = 1,
                    Level = 1,
                    PlayerName = "player1",
                };
                Assert.True(await XTable.Player.Insert(p));
                return true;
            });

            await Edb.Edb.I.DisposeAsync();
        }
        
        [Fact]
        public async Task TestUpdate()
        {
            await Init();
            XTable.Tables.Player.AddListener(new PlayerListenerLevel(), "level");
            XTable.Tables.PlayerHero.AddListener(new PlayerHeroListener(), "heroes");
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                var ph = await XTable.PlayerHero.Update(1);
                p.Level++;
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 1,
                };
                ph.Heroes[h.HeroId] = h;
                var hero = ph.Heroes[1];
                hero.Star++;
                return true;
            });

            await Edb.Edb.I.DisposeAsync();
        }
        
        private class PlayerListenerLevel : IListener
        {
            public Task OnChanged(object key, object val)
            {
                Log.I.Info($"player changed: {key} {val}");
                return Task.CompletedTask;
            }

            public Task OnChanged(object key, object val, string fullVarName, INote? note)
            {
                Log.I.Info($"player changed note: {key} {val} {fullVarName} {note}");
                var player = (Player)val;
                
                Event.Fire(new PlayerEventLevel((long)key)
                {
                    Level = player.Level,
                });
                return Task.CompletedTask;
            }

            public Task OnRemoved(object key, object val)
            {
                Log.I.Info($"player removed: {key} {val}");
                return Task.CompletedTask;
            }
        }

        private class PlayerHeroListener : IListener
        {
            public async Task OnChanged(object key, object val)
            {
                Log.I.Info($"player changed: {key} {val}");
            }

            public async Task OnChanged(object key, object val, string fullVarName, INote? note)
            {
                Log.I.Info($"player changed note: {key} {val} {fullVarName} {note}");
            }

            public async Task OnRemoved(object key, object val)
            {
                Log.I.Info($"player removed: {key} {val}");
            }
        }
    }

    public class PlayerEventLevel : EEvent<long>
    {
        public int Level { get; set; }

        public PlayerEventLevel(long key) : base(key)
        {
        }
    }
    
    public class PlayerEventHandler : IEventHandler
    {
        [Listener(typeof(PlayerEventLevel))]
        public static void OnPlayerLevelUpEvent(IEvent e0)
        {
            var e = (PlayerEventLevel)e0;
            Log.I.Info($"player {e.Key} level up to {e.Level}");
        }
    }
}