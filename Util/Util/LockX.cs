
namespace Evil.Util
{
    public class LockX
    {
        private readonly ReaderWriterLockSlim m_Lock = new();
        
        public void RLock()
        {
            m_Lock.EnterReadLock();
        }

        public void RLock(int timeoutMills)
        {
            if (!m_Lock.TryEnterReadLock(timeoutMills))
                throw new LockTimeoutException();
        }
        
        public bool RTryLock()
        {
            return m_Lock.TryEnterReadLock(0);
        }
        
        public bool RTryLock(int timeoutMills)
        {
            return m_Lock.TryEnterReadLock(timeoutMills);
        }

        public void RUnlock()
        { 
            m_Lock.ExitReadLock();
        }
        
        public void WLock()
        {
            m_Lock.EnterWriteLock();
        }

        public void WLock(int timeoutMills)
        {
            if (!m_Lock.TryEnterWriteLock(timeoutMills))
                throw new LockTimeoutException();
        }
        
        public bool WTryLock()
        {
            return m_Lock.TryEnterWriteLock(0);
        }
        
        public bool WTryLock(int timeoutMills)
        {
            return m_Lock.TryEnterWriteLock(timeoutMills);
        }
        
        public void WUnlock()
        {
            m_Lock.ExitWriteLock();
        }
    }
    
    public class LockTimeoutException : Exception
    {
        public LockTimeoutException() : base()
        {
        }
        public LockTimeoutException(string message) : base(message)
        {
        }
    }
}