using System;
using System.Threading.Tasks;
using Edb;
using Evil.Util;
using XBean;
using Xunit;

namespace Game.Test
{
    public class TransactionTest
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
        public async Task TestBaseCommit()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                p!.Level++;
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var p1 = await XTable.Player.Select(1);
                Assert.Equal(2, p1!.Level);
                return true;
            });
            
            await Edb.Edb.I.DisposeAsync();
        }
        
        [Fact]
        public async Task TestBaseRollback()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                p!.Level++;
                return false;
            });
            await Procedure.Submit(async () =>
            {
                var p1 = await XTable.Player.Select(1);
                Assert.Equal(1, p1!.Level);
                return true;
            });

            await Edb.Edb.I.DisposeAsync();
        }

        [Fact]
        public async Task TestListCommit()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Update(1);
                ph!.Heroes[0].Star++;
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 1,
                    Properties =
                    {
                        Abs = 1,
                        Pct = 2,
                    }
                };
                ph.Heroes.Add(h);
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                Assert.Equal(2, ph!.Heroes.Count);
                Assert.Equal(2, ph.Heroes[0].Star);
                Assert.Equal(1, ph.Heroes[1].Properties.Abs);
                Assert.Equal(2, ph.Heroes[1].Properties.Pct);
                return true;
            });
            await Edb.Edb.I.DisposeAsync();
        }
        
        [Fact]
        public async Task TestListRollback()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Update(1);
                ph!.Heroes[0].Star++;
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 1,
                };
                ph.Heroes.Add(h);
                return false;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                Assert.Equal(1, ph!.Heroes.Count);
                Assert.Equal(1, ph.Heroes[0].Star);
                return true;
            });
            await Edb.Edb.I.DisposeAsync();
        }

        [Fact]
        public async Task TestParentBean()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 1,
                    Properties =
                    {
                        Abs = 1,
                        Pct = 2,
                    }
                };
                ph!.Heroes.Add(h);
                try
                {
                    ph.Heroes.Add(h);
                } catch (Exception e)
                {
                    Assert.True(e is XManagedError);
                    return false;
                }

                return true;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                Assert.Equal(1, ph!.Heroes.Count);
                return true;
            });
            await Edb.Edb.I.DisposeAsync();
        }

        [Fact]
        public async Task TestMapCommit()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 2,
                };
                p!.Heroes[h.HeroId] = h;
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Select(1);
                Assert.Equal(2, p!.Heroes.Count);
                Assert.Equal(2, p.Heroes[2].Star);
                return true;
            });
            await Edb.Edb.I.DisposeAsync();
        }
        
        [Fact]
        public async Task TestMapRollback()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                p.Heroes[1].Star++;
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 2,
                };
                p.Heroes[h.HeroId] = h;
                return false;
            });
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Select(1);
                Assert.Equal(1, p!.Heroes.Count);
                Assert.Equal(1, p.Heroes[1].Star);
                return true;
            });
            await Edb.Edb.I.DisposeAsync();
        }
        
        [Fact]
        public async Task TestVerify()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Select(1);
                try
                {
                    p.Heroes[1].Star++;
                } catch (Exception e)
                {
                    Assert.True(e is XLockLackedError);
                    return false;
                }

                return true;
            });
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Select(1);
                Assert.Equal(1, p!.Heroes[1].Star);
                return true;
            });
            await Edb.Edb.I.DisposeAsync();
        }

        [Fact]
        public async Task TestDeadLock()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                await XTable.Player.Delete(2);
                var p = new XBean.Player()
                {
                    PlayerId = 2,
                    Level = 2,
                    PlayerName = "player2",
                };
                await XTable.Player.Insert(p);
                return true;
            });
            var t1= Procedure.Submit(async () =>
            {
                Log.I.Info("t1 start");
                var p1 = await XTable.Player.Update(1);
                var p2 = await XTable.Player.Update(2);
                p1!.Level++;
                p2!.Level++;
                Log.I.Info("t1 end");
                return true;
            });
            var t2 = Procedure.Submit(async () =>
            {
                Log.I.Info("t2 start");
                var p1 = await XTable.Player.Update(2);
                var p2 = await XTable.Player.Update(1);
                p1!.Level++;
                p2!.Level++;
                Log.I.Info("t2 end");
                return true;
            });
            await Task.WhenAll(t1, t2);
            await Edb.Edb.I.DisposeAsync();
        }
    }
}