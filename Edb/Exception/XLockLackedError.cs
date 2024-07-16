namespace Edb
{
    public class XLockLackedError : XError
    {
        public XLockLackedError(string message) : base(message)
        {
        }
    }
}