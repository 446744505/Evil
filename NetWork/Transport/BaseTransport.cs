using System.Threading;

namespace NetWork.Transport
{
    public abstract class BaseTransport<T> : ITransport where T : TransportConfig
    {
        #region 字段

        protected ISessionMgr m_SessionMgr;
        protected IMessageProcessor m_MessageProcessor;

        private volatile bool m_IsStop;
        private readonly AutoResetEvent m_StopEvent = new(false);

        #endregion

        #region 属性

        protected T Config { get; }

        #endregion

        public BaseTransport(T config)
        {
            Config = config;
            m_SessionMgr = Config.NetWorkFactory.CreateSessionMgr();
            var messageRegister = Config.NetWorkFactory.CreateMessageRegister();
            m_MessageProcessor = new MessageProcessor();
            messageRegister.Register(m_MessageProcessor);
        }

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