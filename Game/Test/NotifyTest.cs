using System.Threading.Tasks;
using Edb;
using Evil.Util;
using XBean;
using Xunit;

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
                var hero = new XBean.Hero()
                {
                    HeroId = 1,
                    Star = 1,
                };
                var p = new Player()
                {
                    PlayerId = 1,
                    Level = 1,
                    PlayerName = "player1",
                };
                p.Heroes[hero.HeroId] = hero;
                Assert.True(await XTable.Player.Insert(p));

                await XTable.PlayerHero.Delete(1);
                var skill = new XBean.HeroSkill()
                {
                    CfgId = 1,
                    Level = 1,
                };
                
                hero.Skills.Add(skill);
                var ph = new PlayerHero()
                {
                    PlayerId = 1,
                };
                var h2 = new XBean.Hero();
                h2.CopyFrom(hero);
                ph.Heroes.Add(h2);
                Assert.True(await XTable.PlayerHero.Insert(ph));

                return true;
            });
        }
        
        [Fact]
        public async Task TestInsert()
        {
            await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
            XTable.Tables.Player.AddListener(new PlayerListener());
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
            XTable.Tables.Player.AddListener(new PlayerListener(), "heroes");
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                p.Level++;
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 1,
                };
                p.Heroes[h.HeroId] = h;
                return true;
            });

            await Edb.Edb.I.DisposeAsync();
        }
        
        private class PlayerListener : IListener
        {
            public async Task OnChanged(object key, object val)
            {
                Log.I.Info($"player changed: {key} {val}");
            }

            public async Task OnChanged(object key, object val, string fullVarName, INote? note)
            {
                var p = await XTable.Player.Select(long.Parse(key.ToString()!));
                Log.I.Info($"player changed note: {key} {val} {fullVarName} {note}");
            }

            public async Task OnRemoved(object key, object val)
            {
                Log.I.Info($"player removed: {key} {val}");
            }
        }
    }
}