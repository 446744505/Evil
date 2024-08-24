namespace Evil.Util
{
    public class Elapse
    {
        public long Start { get; private set; } = Time.Now;
        
        public long Elapsed() => Time.Now - Start;
        
        public void Reset() => Start = Time.Now;
        
        public long ElapsedAndReset()
        {
            var now = Time.Now;
            var elapsed = now - Start;
            Start = now;
            return elapsed;
        }
    }
}