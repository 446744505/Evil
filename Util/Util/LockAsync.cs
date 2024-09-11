using Nito.AsyncEx;

namespace Evil.Util
{
    public class LockAsync
    {
        private readonly AsyncReaderWriterLock m_Lock = new();

        public async Task<IDisposable> RLockAsync()
        {
            return await m_Lock.ReaderLockAsync();
        }
        
        public IDisposable RLock()
        {
            return m_Lock.ReaderLock();
        }
        
        public async Task<IDisposable> RLockAsync(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                return await m_Lock.ReaderLockAsync(cts.Token);
            } catch (TaskCanceledException)
            {
                throw new LockTimeoutException();
            }
            finally
            {
                cts.Dispose();
            }
        }
        
        public IDisposable RLock(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                return m_Lock.ReaderLock(cts.Token);
            } catch (TaskCanceledException)
            {
                throw new LockTimeoutException();
            }
            finally
            {
                cts.Dispose();
            }
        }
        
        /// <summary>
        /// 用超时模拟的try lock，不能用在性能敏感的场景
        /// </summary>
        /// <returns></returns>
        public async Task<IDisposable?> RTryLockAsync()
        {
            // 用5ms尝试获取锁
            try {
                return await RLockAsync(5);
            } catch (LockTimeoutException) {
                return null;
            }
        }
        
        public void RUnlock(IDisposable release)
        {
            release.Dispose();
        }
        
        public async Task<IDisposable> WLockAsync()
        {
            return await m_Lock.WriterLockAsync();
        }
        
        public IDisposable WLock()
        {
            return m_Lock.WriterLock();
        }
        
        public async Task<IDisposable> WLockAsync(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                return await m_Lock.WriterLockAsync(cts.Token);
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
        
        public IDisposable WLock(int timeoutMills)
        {
            var cts = new CancellationTokenSource(timeoutMills);
            try
            {
                return m_Lock.WriterLock(cts.Token);
            } catch (TaskCanceledException)
            {
                throw new LockTimeoutException();
            }
            finally
            {
                cts.Dispose();
            }
        }
        
        /// <summary>
        /// 用超时模拟的try lock，不能用在性能敏感的场景
        /// </summary>
        /// <returns></returns>
        public async Task<IDisposable?> WTryLockAsync()
        {
            // 用5ms尝试获取锁
            try {
                return await WLockAsync(5);
            } catch (LockTimeoutException) {
                return null;
            }
        }

        public void WUnlock(IDisposable release)
        {
            release.Dispose();
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