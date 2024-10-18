
using System.Threading.Tasks;
using Edb;
using Evil.Event;
using Evil.Util;
using XBean;
using XListener;
using Xunit;

namespace Game.Test
{
    public class NotifyTest
    {
        private async Task Init()
        {
            Edb.Edb.I.Start(new Config(), XTable.Tables.All);
            await Procedure.Submit(() =>
            {
                XTable.Player.Delete(1);

                var p = new Player()
                {
                    PlayerId = 1,
                    Level = 1,
                    PlayerName = "player1",
                };
                Assert.True(XTable.Player.Insert(p));

                XTable.PlayerHero.Delete(1);
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
                Assert.True(XTable.PlayerHero.Insert(ph));

                return true;
            });
        }

        [Fact]
        public async Task TestInsert()
        {
            Edb.Edb.I.Start(new Config(), XTable.Tables.All);
            XTable.Tables.Player.AddListener(new PlayerLevelListener(), "level");
            await Procedure.Submit(() =>
            {
                XTable.Player.Delete(1);
                var p = new Player()
                {
                    PlayerId = 1,
                    Level = 1,
                    PlayerName = "player1",
                };
                Assert.True(XTable.Player.Insert(p));
                return true;
            });

            Edb.Edb.I.Dispose();
        }

        [Fact]
        public async Task TestUpdate()
        {
            await Init();
            XTable.Tables.Player.AddListener(new PlayerLevelListener(), "level");
            XTable.Tables.PlayerHero.AddListener(new PlayerHeroHeroesListener(), "heroes");
            await Procedure.Submit(() =>
            {
                var p = XTable.Player.Update(1);
                var ph = XTable.PlayerHero.Update(1);
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

            Edb.Edb.I.Dispose();
        }

        public class PlayerEventHandler : IEventHandler
        {
            [Listener(typeof(PlayerLevelEvent))]
            public static void OnPlayerLevelUpEvent(IEvent e0)
            {
                var e = (PlayerLevelEvent)e0;
                Log.I.Info($"player {e.Key} level up to {e.Level}");
            }
        }
        
        public class PlayerHeroEventHandler : IEventHandler
        {
            [Listener(typeof(PlayerHeroHeroesEvent))]
            public static void OnPlayerHeroEvent(IEvent e0)
            {
                var e = (PlayerHeroHeroesEvent)e0;
                Log.I.Info(e.IsAdd ? $"player {e.Key} add hero {e.MKey}" : $"player {e.Key} remove hero {e.MKey}");
            }
        }
    }
}