using System;

namespace NetWork
{
    public class TimeoutException : Exception
    {
        public TimeoutException(string message) : base(message)
        {
        }
    }
}