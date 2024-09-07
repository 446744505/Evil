
namespace Game.Test
{
    public static class Test
    {
        public static void Init()
        {
            new TransactionTest().TestWhenCommit().Wait();
        }
    }
}