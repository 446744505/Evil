using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork.Proto;
using Nito.AsyncEx;

namespace NetWork.Transport
{
    public abstract class BaseTransport<T> : ITransport where T : TransportConfig
    {
        #region 字段

        protected ISessionMgr m_SessionMgr = null!;

        private volatile bool m_IsStop;
        private readonly RpcMgr m_RpcMgr = new();
        private readonly AsyncCountdownEvent m_StopEvent = new(1);

        #endregion

        #region 属性

        protected T Config { get; }
        internal RpcMgr RpcMgr => m_RpcMgr;

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
        
        protected void OnStopped()
        {
            m_IsStop = true;
        }

        protected async Task BaseDispose(IChannel channel)
        {
            // 关闭连接
            await channel.CloseAsync();
            // 关闭rpc，上面的连接已经关了，如果这时候client回调处理里还有消息发送？
            await RpcMgr.DisposeAsync();
            // 最后关闭线程池
            await Config.Executor.DisposeAsync();
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

        TransportConfig ITransport.Config()
        {
            return Config;
        }

        RpcMgr ITransport.RpcMgr()
        {
            return m_RpcMgr;
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