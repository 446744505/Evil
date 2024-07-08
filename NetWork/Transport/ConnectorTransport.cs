using System.Net;
using DotNetty.Codecs;
using DotNetty.Codecs.Protobuf;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NetWork.Codec;
using NetWork.Handler;

namespace NetWork.Transport
{
    public class ConnectorTransport : BaseTransport
    {
        private ConnectorTransportConfig Config { get; }

        public ConnectorTransport(ConnectorTransportConfig config)
        {
            Config = config;
        }

        public override async void Start()
        {
            var group = new MultithreadEventLoopGroup(Config.WorkerCount);
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap.Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddLast(new LoggingHandler());
                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 
                            Messages.HeaderSize, 0, Messages.HeaderSize));
                        pipeline.AddLast(new LengthFieldPrepender(Messages.HeaderSize));
                        pipeline.AddLast(new MessageDecode());
                        pipeline.AddLast(new MessageEncode());
                        pipeline.AddLast(new LogicHandler());
                    }));
                var channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Config.Host), Config.Port));
                Log.I.Info($"connector connect to {Config.Host}:{Config.Port}");
                // 等待关闭
                WaitStop();
                // 关闭连接
                await channel.CloseAsync();
                Log.I.Info($"connector stop at {Config.Host}:{Config.Port}");
            }
            finally
            {
                // 释放资源
                await group.ShutdownGracefullyAsync();
                Stopped();
            }
        }
    }
}