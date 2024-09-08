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
                ph!.Heroes[1].Star++;
                var skill = new XBean.HeroSkill()
                {
                    CfgId = 2,
                    Level = 2,
                };
                ph.Heroes[1].Skills.Add(skill);
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                var h = ph!.Heroes[1];
                Assert.Equal(2, h.Skills.Count);
                Assert.Equal(2, ph.Heroes[1].Star);
                Assert.Equal(1, h.Skills[0].CfgId);
                Assert.Equal(2, h.Skills[1].CfgId);
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
                ph!.Heroes[1].Star++;
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 1,
                };
                ph.Heroes[h.HeroId] = h;
                return false;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                Assert.Equal(1, ph!.Heroes.Count);
                Assert.Equal(1, ph.Heroes[1].Star);
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
                ph.Heroes[h.HeroId] = h;
                try
                {
                    ph.Heroes[h.HeroId] = h;
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
                var ph = await XTable.PlayerHero.Update(1);
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 2,
                };
                ph!.Heroes[h.HeroId] = h;
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                Assert.Equal(2, ph!.Heroes.Count);
                Assert.Equal(2, ph.Heroes[2].Star);
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
                var ph = await XTable.PlayerHero.Update(1);
                ph.Heroes[1].Star++;
                var h = new XBean.Hero()
                {
                    HeroId = 2,
                    Star = 2,
                };
                ph.Heroes[h.HeroId] = h;
                return false;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                Assert.Equal(1, ph!.Heroes.Count);
                Assert.Equal(1, ph.Heroes[1].Star);
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
                var ph = await XTable.PlayerHero.Select(1);
                try
                {
                    ph.Heroes[1].Star++;
                } catch (Exception e)
                {
                    Assert.True(e is XLockLackedError);
                    return false;
                }

                return true;
            });
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(1);
                Assert.Equal(1, ph!.Heroes[1].Star);
                return true;
            });
            await Edb.Edb.I.DisposeAsync();
        }

        [Fact]
        public async Task TestCallCommitCommit()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                p!.Level++;
                var subR = await Procedure.Call(async () =>
                {
                    var p1 = await XTable.Player.Update(1);
                    p1!.Level++;
                    return true;
                });
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var p1 = await XTable.Player.Select(1);
                Assert.Equal(3, p1!.Level);
                return true;
            });
        }
        
        [Fact]
        public async Task TestCallCommitRollback()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                p!.Level++;
                var subR = await Procedure.Call(async () =>
                {
                    var p1 = await XTable.Player.Update(1);
                    p1!.Level++;
                    return false;
                });
                return true;
            });
            await Procedure.Submit(async () =>
            {
                var p1 = await XTable.Player.Select(1);
                Assert.Equal(2, p1!.Level);
                return true;
            });
        }
        
        [Fact]
        public async Task TestCallRollbackCommit()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                var p = await XTable.Player.Update(1);
                p!.Level++;
                var subR = await Procedure.Call(async () =>
                {
                    var p1 = await XTable.Player.Update(1);
                    p1!.Level++;
                    return true;
                });
                return false;
            });
            await Procedure.Submit(async () =>
            {
                var p1 = await XTable.Player.Select(1);
                Assert.Equal(1, p1!.Level);
                return true;
            });
        }
        
        public struct WhenCommit : Procedure
        {
            private readonly int m_Num;

            public WhenCommit(int num)
            {
                m_Num = num;
            }

            public async Task<bool> Process()
            {
                Log.I.Info($"Start WhenCommit {m_Num}");
                await Task.Delay(50);
                // Transaction.AddSavepointTask(new WhenCommit(m_Num+1), null);
                Log.I.Info($"End WhenCommit {m_Num}");
                return true;
            }
        }

        [Fact]
        public async Task TestWhenCommit()
        {
            await Init();
            await Procedure.Submit(async () =>
            {
                Log.I.Info($"Start TestWhenCommit");
                // Transaction.AddSavepointTask(new WhenCommit(1), null);
                Procedure.Execute(new WhenCommit(1));
                // Procedure.Execute(new WhenCommit(2));
                Log.I.Info($"End TestWhenCommit");
                return true;
            });
            await Task.Delay(2000);
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