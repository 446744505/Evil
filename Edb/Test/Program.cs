
using Evil.Util;

namespace Edb.Test
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var edbTest = new EdbTest();
            await edbTest.TestAdd();
            await edbTest.TestUpdate();
        }
    }
}