using Evil.Util;
using Xunit;

namespace Edb.Test
{
    public class ProcedureTest
    {
        private async Task Init()
        {
            await Edb.I.Start(new Config(), new List<BaseTable>());
        }
        [Fact]
        public async Task TestNomal()
        {
            await Init();
            
            var pTrue = new PNomal(true);
            var r1 = await Procedure.Submit(pTrue);
            Assert.True(r1.IsSuccess);
            var pFalse = new PNomal(false);
            var r2 = await Procedure.Submit(pFalse);
            Assert.False(r2.IsSuccess);
        }

        [Fact]
        public async Task TestExecute()
        {
            await Init();
            var list = new List<int>();
            var p = new PExecute(list);
            Procedure.Execute(p);
            Assert.Equal(list.Count, 0);
            await Task.Delay(500);
            Assert.Equal(list.Count, 0);
            await Task.Delay(1000);
            Assert.Equal(list.Count, 1);
        }

        [Fact]
        public async Task TestRetryFail()
        {
            await Init();
            Edb.I.Config.LockTimeoutMills = 1000;
            var list = new List<int>();
            var lockey = Lockeys.GetLockey(1, 1);
            await lockey.RLock();
            var p = new PRetryFail(list, lockey);
            var r = await Procedure.Submit(p);
            Assert.False(r.IsSuccess);
            Assert.True(r.Exception is LockTimeoutException);
            
            Assert.Equal(list.Count, 0);
        }


        [Fact]
        public async Task TestRetryFail1()
        {
            await Init();
            Edb.I.Config.LockTimeoutMills = 1000;
            var list = new List<int>();
            var lockey = Lockeys.GetLockey(1, 1);
            await lockey.RLock();
            var p = new PRetryFail(list, lockey);
            Exception? exception = null;
            Procedure.Execute(p, (_, r) => { exception = r.Exception; });
            await Task.Delay(5000);
            Assert.True(exception is LockTimeoutException);
            Assert.Equal(list.Count, 0);
        }

        [Fact]
        public async Task TestRetrySuccess()
        {
            await Init();
            Edb.I.Config.LockTimeoutMills = 1000;
            var list = new List<int>();
            var lockey = Lockeys.GetLockey(1, 1);
            var p = new PRetrySuccess(list, lockey);
            new Thread(async _ =>
            {
                var release = await lockey.RLock();
                Thread.Sleep(1000);
                lockey.RUnlock(release);
            }).Start();
            var r = await Procedure.Submit(p);
            Assert.True(r.IsSuccess);
            Assert.Equal(list.Count, 1);
        }

        [Fact]
        public async Task TestFunc()
        {
            await Init();
            var r1 = await Procedure.Submit( () => true);
            Assert.True(r1.IsSuccess);
            var num = 0;
            var r2 = await Procedure.Call(() =>
            {
                num++;
                return true;
            });
            Assert.True(r2.IsSuccess);
            Assert.Equal(1, num);
            Procedure.Execute(() =>
            {
                num++;
                return true;
            });
            await Task.Delay(500);
            Assert.Equal(2, num);
        }
        
        private class PNomal : Procedure
        {
            public readonly bool m_R;

            public PNomal(bool r)
            {
                m_R = r;
            }

            public Task<bool> Process()
            {
                return Task.FromResult(m_R);
            }
        }
        
        private class PExecute : Procedure
        {
            private readonly List<int> m_List;

            public PExecute(List<int> list)
            {
                m_List = list;
            }

            public async Task<bool> Process()
            {
                await Task.Delay(1000);
                m_List.Add(1);
                return true;
            }
        }

        private class PRetryFail : Procedure
        {
            private readonly List<int> m_List;
            private readonly Lockey m_Lockey;

            public PRetryFail(List<int> list, Lockey lockey)
            {
                m_List = list;
                m_Lockey = lockey;
            }

            public async Task<bool> Process()
            {
                await m_Lockey.WLock(Edb.I.Config.LockTimeoutMills);
                m_List.Add(1);
                return true;
            }
        }
        
        private class PRetrySuccess : Procedure
        {
            private readonly List<int> m_List;
            private readonly Lockey m_Lockey;

            public PRetrySuccess(List<int> list, Lockey lockey)
            {
                m_List = list;
                m_Lockey = lockey;
            }

            public async Task<bool> Process()
            {
                await m_Lockey.WLock(Edb.I.Config.LockTimeoutMills);
                m_List.Add(1);
                return true;
            }
        }
    }
}