using Xunit;

namespace Edb.Test
{
    public class LockeysTest
    {
        [Fact]
        public void TestGc()
        {
            for (var i = 0; i < 9999999; i++)
            {
                Assert.NotNull(Lockeys.GetLockey(1, 1));
            }
            // 强制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            for (var i = 0; i < 9999999; i++)
            {
                Assert.NotNull(Lockeys.GetLockey(1, 1));
            }
        }
    }
}