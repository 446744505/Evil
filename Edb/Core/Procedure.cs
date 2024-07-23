namespace Edb
{
    public abstract class Procedure
    {
        private volatile bool m_Success;
        private volatile Exception m_Exception;
        public string Name => GetType().Name;
        internal IsolationLevel IsolationLevel { get; }
        
        internal bool Success
        {
            get => m_Success;
            set => m_Success = value;
        }

        internal Exception Exception
        {
            get => m_Exception;
            set => m_Exception = value;
        }

        internal bool Call()
        {
            
        }

    }
}