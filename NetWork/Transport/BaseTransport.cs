namespace NetWork.Transport
{
    public abstract class BaseTransport : ITransport
    {
        private volatile bool m_IsStop;
        private readonly AutoResetEvent m_StopEvent = new(false);

        protected void WaitStop()
        {
            m_StopEvent.WaitOne();
        }
        
        protected void Stopped()
        {
            m_IsStop = true;
        }
    
        public void Dispose()
        {
            m_StopEvent.Set();
            while (!m_IsStop)
            {
                Thread.Sleep(1000);
            }
        }
    
        public abstract void Start();
    }
}