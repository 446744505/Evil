namespace Edb
{
    public class XError : Exception
    {
        public XError(string message) : base(message)
        {
        }
        
        public XError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}