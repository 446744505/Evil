namespace Evil.Util
{
    public class Time
    {
        public static long Now => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}