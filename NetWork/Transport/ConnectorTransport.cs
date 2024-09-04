
using System.Net;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Evil.Util;
using NetWork.Codec;
using NetWork.Handler;

namespace NetWork.Transport
{
    public class ConnectorTransport : BaseTransport<ConnectorTransportConfig>
    {
        private readonly Bootstrap m_Bootstrap = new();
        private readonly ReConnectHandler? m_ReConnectHandler;
        private IChannel m_Channel = null!;
        public ConnectorTransport(ConnectorTransportConfig config) : base(config)
        {
            if (config.ReConnectDelay > 0)
            {
                m_ReConnectHandler = new ReConnectHandler(this);
            }
        }

        protected override async Task Start0()
        {
            var group = new MultithreadEventLoopGroup(Config.WorkerCount);
            try
            {
                m_Bootstrap.Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Option(ChannelOption.SoKeepalive, false)
                    .Option(ChannelOption.SoRcvbuf, Config.SoRcvbuf)
                    .Option(ChannelOption.SoSndbuf, Config.SoSndbuf)
                    .Option(ChannelOption.WriteBufferLowWaterMark, Config.OutBufferSize)
                    .Option(ChannelOption.WriteBufferHighWaterMark, Config.OutBufferSize)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        AddChannelHandler(channel.Pipeline);
                    }));
               
                DoConnect();
                
                // 等待关闭
                await WaitStop();
                await BaseDispose(m_Channel);
                Log.I.Info($"connector stop at {Config.Host}:{Config.Port}");
            }
            finally
            {
                // 释放资源
                await group.ShutdownGracefullyAsync();
                OnStopped();
            }
        }

        private void DoConnect()
        {
            var task = m_Bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Config.Host), Config.Port));
            task.ContinueWith(_ =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    m_Channel = task.Result;
                    OnStarted0(m_Channel);
                    Log.I.Info($"connector connect to {this} success");
                }
                else
                {
                    Log.I.Error($"connector connect to {this} fail");
                    TryConnect();
                }
            });
        }

        public void TryConnect()
        {
            if (IsStop)
            {
                Log.I.Warn($"transport {this} is stopped, give up reconnect");
                return;
            }

            if (Config.ReConnectDelay <= 0)
            {
                Log.I.Warn($"{this} will not reconnect");
                return;
            }
            Log.I.Warn($"{this} will reconnect");
            Config.Executor.Delay(DoConnect, Config.ReConnectDelay * 1000);
        }
        
        protected virtual void AddChannelHandler(IChannelPipeline pipeline)
        {
            if (m_ReConnectHandler != null)
                pipeline.AddLast(m_ReConnectHandler);
            pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 
                Messages.HeaderSize, 0, Messages.HeaderSize));
            pipeline.AddLast(new LengthFieldPrepender(Messages.HeaderSize));
            pipeline.AddLast(new MessageDecode(Config.MessageProcessor));
            pipeline.AddLast(new MessageEncode());
            pipeline.AddLast(new LogicHandler(this, m_SessionMgr));
        }

        public override string ToString()
        {
            return $"{Config.Host}:{Config.Port}";
        }
    }
}