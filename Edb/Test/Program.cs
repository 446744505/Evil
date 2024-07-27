
namespace Edb.Test
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var tableTest = new TableTest();
            await tableTest.TestCallback();
        }
    }
}