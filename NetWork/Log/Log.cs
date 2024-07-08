namespace NetWork
{
    public class Log
    {
        public static Log I { get; } = new();
        private Log()
        {
        }

        public void Info(string log)
        {
            Console.WriteLine(log);
        }
    }
}