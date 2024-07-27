namespace Evil.Util
{
    public class Elapse
    {
        public long Start { get; private set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        public long Elapsed() => DateTimeOffset.Now.ToUnixTimeMilliseconds() - Start;
        
        public void Reset() => Start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        public long ElapsedAndReset()
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var elapsed = now - Start;
            Start = now;
            return elapsed;
        }
    }
}