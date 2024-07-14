namespace Edb
{
    public class LockTimeoutException : Exception
    {
        public LockTimeoutException(string message) : base(message)
        {
        }
    }
}