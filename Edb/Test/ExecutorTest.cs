
using Evil.Util;
using Xunit;

namespace Edb.Test
{
    public class ExecutorTest
    {
        [Fact]
        public async Task TestTick()
        {
            var edb = Edb.I;
            var num = 0;
            edb.Executor.Tick(() =>
            {
                num++;
            }, 0, 500);
            await Task.Delay(5010);
            Assert.Equal(10, num);
        }
        
        [Fact]
        public async Task TestTickShutDown()
        {
            var executor = Edb.I.Executor;
            var count = 100;
            var executed = 0;
            for (var i = 0; i < count; i++)
            {
                var j = i;
                executor.Tick(() =>
                {
                    Interlocked.Increment(ref executed);
                }, 1, 999999); // 设置一个很大的周期，保证不会开始第二次执行
            }
            
            // 保证定时器都启动了
            await Task.Delay(100);
            executor.Dispose();
            Assert.Equal(count, executed);
        }
        
        [Fact]
        public async Task TestExecute()
        {
            var executor = Edb.I.Executor;
            var num = 0;
            await executor.SubmitAsync(() => num++);
            Assert.Equal(1, num);
        }
        
        [Fact]
        public void TestExecuteShutDown()
        {
            var executor = Edb.I.Executor;
            var count = 100;
            var executed = 0;
            for (var i = 0; i < count; i++)
            {
                executor.Execute(() =>
                {
                    Interlocked.Increment(ref executed);
                });
            }
            
            executor.Dispose();
            Assert.Equal(count, executed);
        }
    }
}