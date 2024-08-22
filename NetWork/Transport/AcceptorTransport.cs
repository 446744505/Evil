
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Evil.Util;
using NetWork.Codec;
using NetWork.Handler;

namespace NetWork.Transport
{
    public class AcceptorTransport : BaseTransport<AcceptorTransportConfig>
    {
        public AcceptorTransport(AcceptorTransportConfig config) : base(config)
        {
        }

        protected override async Task Start0()
        {
            var bossGroup = new DispatcherEventLoopGroup();
            var workerGroup = new WorkerEventLoopGroup(bossGroup);
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup)
                    .Channel<TcpServerChannel>()
                    .Option(ChannelOption.SoBacklog, Config.Backlog)
                    .ChildOption(ChannelOption.TcpNodelay, true)
                    .ChildOption(ChannelOption.SoKeepalive, false)
                    .ChildOption(ChannelOption.SoRcvbuf, Config.SoRcvbuf)
                    .ChildOption(ChannelOption.SoSndbuf, Config.SoSndbuf)
                    .ChildOption(ChannelOption.WriteBufferLowWaterMark, Config.OutBufferSize)
                    .ChildOption(ChannelOption.WriteBufferHighWaterMark, Config.OutBufferSize)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        AddChildHandler(channel.Pipeline);
                    }));
                
                var channel = await bootstrap.BindAsync(Config.Port);
                Log.I.Info($"acceptor start at {Config.Port}");
                // 等待关闭
                await WaitStop();
                // 关闭连接
                await channel.CloseAsync();
                await Config.Executor.DisposeAsync();
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

        protected virtual void AddChildHandler(IChannelPipeline pipeline)
        {
            pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 
                Messages.HeaderSize, 0, Messages.HeaderSize));
            pipeline.AddLast(new LengthFieldPrepender(Messages.HeaderSize));
            pipeline.AddLast(new MessageDecode(m_MessageProcessor));
            pipeline.AddLast(new MessageEncode());
            pipeline.AddLast(new LogicHandler(Config.NetWorkFactory!, m_SessionMgr, Config.Dispatcher!));
        }
    }
}