using Evil.Util;

namespace Evil.Switcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.I.UnhandledException().UnobservedTaskException();
            
            CmdLine.Init(args);
            Switcher.I.Start();
        }

    }
}