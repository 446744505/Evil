using Nito.AsyncEx;

namespace Evil.Util
{
    public class LockAsync
    {
        private readonly AsyncReaderWriterLock m_Lock = new();

        private IDisposable m_Release;
        
        public async Task RLockAsync()
        {
            m_Release = await m_Lock.ReaderLockAsync();
        }
        
        public void RLock()
        {
            m_Release = m_Lock.ReaderLock();
        }
        
        public async Task RLockAsync(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                m_Release = await m_Lock.ReaderLockAsync(cts.Token);
            } catch (TaskCanceledException)
            {
                throw new LockTimeoutException();
            }
            finally
            {
                cts.Dispose();
            }
        }
        
        public void RLock(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                m_Release = m_Lock.ReaderLock(cts.Token);
            } catch (TaskCanceledException)
            {
                throw new LockTimeoutException();
            }
            finally
            {
                cts.Dispose();
            }
        }
        
        public void RUnlock()
        {
            m_Release.Dispose();
        }
        
        public async Task WLockAsync()
        {
            m_Release = await m_Lock.WriterLockAsync();
        }
        
        public void WLock()
        {
            m_Release = m_Lock.WriterLock();
        }
        
        public async Task WLockAsync(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                m_Release = await m_Lock.WriterLockAsync(cts.Token);
            }
            catch (TaskCanceledException)
            {
                throw new LockTimeoutException();
            }
            finally
            {
                cts.Dispose();
            }
        }
        
        public void WLock(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                m_Release = m_Lock.WriterLock(cts.Token);
            } catch (TaskCanceledException)
            {
                throw new LockTimeoutException();
            }
            finally
            {
                cts.Dispose();
            }
        }

        public void WUnlock()
        {
            m_Release.Dispose();
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