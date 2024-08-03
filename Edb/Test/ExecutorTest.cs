using Xunit;

namespace Edb.Test
{
    public class ExecutorTest
    {
        [Fact]
        public async void TestTick()
        {
            var edb = Edb.I;
            var num = 0;
            edb.Executor.Tick(() =>
            {
                num++;
            }, 0, 500);
            await Task.Delay(5010);
            Assert.Equal(11, num);
        }
        
        [Fact]
        public async void TestTickShutDown()
        {
            var edb = Edb.I;
            var count = 100;
            var executed = 0;
            for (var i = 0; i < count; i++)
            {
                edb.Executor.Tick(() =>
                {
                    // 模拟执行很长时间
                    Task.Delay(1000).Wait();
                    Interlocked.Increment(ref executed);
                }, 1, 999999); // 设置一个很大的周期，保证不会开始第二次执行
            }
            
            // 保证定时器都启动了
            await Task.Delay(10);
            await edb.DisposeAsync();
            Assert.Equal(count, executed);
        }
        
        [Fact]
        public async void TestExecute()
        {
            var edb = Edb.I;
            var num = 0;
            await edb.Executor.ExecuteAsync(() =>
            {
                num++;
            });
            Assert.Equal(1, num);
        }
        
        [Fact]
        public async void TestExecuteShutDown()
        {
            var edb = Edb.I;
            var count = 100;
            var executed = 0;
            for (var i = 0; i < count; i++)
            {
                edb.Executor.ExecuteAsync(() =>
                {
                    // 模拟执行很长时间
                    Task.Delay(1000).Wait();
                    Interlocked.Increment(ref executed);
                });
            }
            
            await edb.DisposeAsync();
            Assert.Equal(count, executed);
        }
    }
}