using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using NetWork.Codec;
using NetWork.Handler;

namespace NetWork.Transport
{
    public class AcceptorTransport : BaseTransport<AcceptorTransportConfig>
    {
        public AcceptorTransport(AcceptorTransportConfig config) : base(config)
        {
        }

        public override async void Start()
        {
            var bossGroup = new DispatcherEventLoopGroup();
            var workerGroup = new WorkerEventLoopGroup(bossGroup);
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup)
                    .Channel<TcpServerChannel>()
                    .Option(ChannelOption.SoBacklog, Config.Backlog)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 
                            Messages.HeaderSize, 0, Messages.HeaderSize));
                        pipeline.AddLast(new LengthFieldPrepender(Messages.HeaderSize));
                        pipeline.AddLast(new MessageDecode(m_MessageProcessor));
                        pipeline.AddLast(new MessageEncode());
                        pipeline.AddLast(new LogicHandler(Config.NetWorkFactory, m_SessionMgr));
                    }));
                
                var channel = await bootstrap.BindAsync(Config.Port);
                Log.I.Info($"acceptor start at {Config.Port}");
                // 等待关闭
                WaitStop();
                // 关闭连接
                await channel.CloseAsync();
                Log.I.Info($"acceptor stop at {Config.Port}");
            }
            finally
            {
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(),
                    workerGroup.ShutdownGracefullyAsync());
                Stopped();
            }
        }
    }
}