using Xunit;

namespace Edb.Test
{
    public class LockeysTest
    {
        [Fact]
        public void TestGc()
        {
            var ctx = TransactionCtx.Create().Start();
            for (var i = 0; i < 9999; i++)
            {
                Assert.NotNull(Lockeys.GetLockey(i, i, ctx));
            }
            // 强制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            for (var i = 0; i < 9999; i++)
            {
                Assert.NotNull(Lockeys.GetLockey(i, i, ctx));
            }
        }
    }
}