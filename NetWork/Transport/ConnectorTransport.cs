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
        public ConnectorTransport(ConnectorTransportConfig config) : base(config)
        {
        }

        protected override async Task Start0()
        {
            var group = new MultithreadEventLoopGroup(Config.WorkerCount);
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap.Group(group)
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
                var channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Config.Host), Config.Port));
                Log.I.Info($"connector connect to {Config.Host}:{Config.Port}");
                // 等待关闭
                await WaitStop();
                await BaseDispose(channel);
                Log.I.Info($"connector stop at {Config.Host}:{Config.Port}");
            }
            finally
            {
                // 释放资源
                await group.ShutdownGracefullyAsync();
                OnStopped();
            }
        }
        
        protected virtual void AddChannelHandler(IChannelPipeline pipeline)
        {
            pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 
                Messages.HeaderSize, 0, Messages.HeaderSize));
            pipeline.AddLast(new LengthFieldPrepender(Messages.HeaderSize));
            pipeline.AddLast(new MessageDecode(Config.MessageProcessor));
            pipeline.AddLast(new MessageEncode());
            pipeline.AddLast(new LogicHandler(this, m_SessionMgr));
        }
    }
}