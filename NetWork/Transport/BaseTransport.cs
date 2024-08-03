using System.Threading;
using System.Threading.Tasks;
using NetWork.Proto;

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
            if (Config.Dispatcher == null)
                Config.Dispatcher = new MessgeDispatcher(Config.Executor);
            var messageRegister = Config.NetWorkFactory.CreateMessageRegister();
            m_MessageProcessor = new MessageProcessor();
            RegisterMessages();
            messageRegister.Register(m_MessageProcessor);
        }

        private void RegisterMessages()
        {
            m_MessageProcessor.Register(MessageId.RpcResponse, () => new RpcResponse());
            RegisterExtMessages();
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
        
        public void Start()
        {
            // 直接启动一个线程，不阻塞主线程
            Task.Factory.StartNew(Start0, TaskCreationOptions.LongRunning);
        }
        
        protected abstract void Start0();

        public virtual void RegisterExtMessages()
        {
        }
    }
}