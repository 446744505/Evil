namespace Edb
{
    public interface IProcedure
    {
        public virtual int RetryDelay => -1;
        public virtual int RetryTimes => -1;
        public virtual bool RetrySerial => false;
        public string Name => GetType().Name;

        public interface IResult
        {
            bool IsSuccess { get; }
            Exception? Exception { get; }
        }
        
        public struct ResultImpl : IResult
        {
            private readonly bool m_IsSuccess;
            private readonly Exception? m_Exception;

            public ResultImpl(bool isSuccess, Exception? exception)
            {
                m_IsSuccess = isSuccess;
                m_Exception = exception;
            }

            public bool IsSuccess => m_IsSuccess;
            public Exception? Exception => m_Exception;
        }
    }
}