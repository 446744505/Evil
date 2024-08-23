using System;
using System.Threading;
using System.Threading.Tasks;
using Evil.Util;
using NetWork.Proto;
using Nito.AsyncEx;

namespace NetWork.Transport
{
    public abstract class BaseTransport<T> : ITransport where T : TransportConfig
    {
        #region 字段

        protected ISessionMgr m_SessionMgr;

        private volatile bool m_IsStop;
        private readonly AsyncCountdownEvent m_StopEvent = new(1);

        #endregion

        #region 属性

        protected T Config { get; }

        #endregion

        public BaseTransport(T config)
        {
            Config = config;
        }

        private void RegisterMessages()
        {
            Config.MessageProcessor.Register(MessageId.RpcResponse, () => new RpcResponse());
            RegisterExtMessages();
        }

        protected async Task WaitStop()
        {
            await m_StopEvent.WaitAsync();
        }
        
        protected void Stopped()
        {
            m_IsStop = true;
        }
    
        public void Dispose()
        {
            Log.I.Info("transport start stop");
            m_StopEvent.Signal();
            while (!m_IsStop)
            {
                Thread.Sleep(1000);
            }
            Log.I.Info("transport stop");
        }

        public void Start()
        {
            m_SessionMgr = Config.NetWorkFactory!.CreateSessionMgr();
            Config.Dispatcher ??= new MessageDispatcher(Config.Executor);
            var messageRegister = Config.NetWorkFactory.CreateMessageRegister();
            var messageProcessor = Config.NetWorkFactory.CreateMessageProcessor(Config.Pvid);
            Config.MessageProcessor = messageProcessor;
            RegisterMessages();
            messageRegister.Register(messageProcessor);
            
            Task.Run(async () =>
            {
                await Start0();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Log.I.Error(task.Exception);
                    Environment.Exit(-1);
                }
            });
        }
        
        protected abstract Task Start0();

        public virtual void RegisterExtMessages()
        {
        }
    }
}