using System;

namespace NetWork
{
    public class NetWorkException : Exception
    {
        public NetWorkException(string message) : base(message)
        {
        }
    }
}