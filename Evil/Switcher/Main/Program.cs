namespace Evil.Switcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CmdLine.Init(args);
            Switcher.I.Start();
        }
    }
}