using DotNetty.Transport.Channels;

namespace NetWork.Handler
{
    internal class LogicHandler : SimpleChannelInboundHandler<Message>
    {
        private Session m_Session;
        
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            m_Session = new Session(ctx);
            base.ChannelActive(ctx);
            Log.I.Info($"add session {m_Session}");
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Log.I.Info($"remove session {m_Session}");
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            Console.WriteLine(msg);
        }
    }
}