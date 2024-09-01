using DotNetty.Transport.Channels;
using NetWork.Transport;

namespace NetWork.Handler
{
    public class ReConnectHandler : ChannelHandlerAdapter
    {
        private readonly ConnectorTransport m_Transport;

        public override bool IsSharable => true;

        public ReConnectHandler(ConnectorTransport transport)
        {
            m_Transport = transport;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            m_Transport.TryConnect();
        }
    }
}