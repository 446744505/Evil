using DotNetty.Transport.Channels;
using NetWork;
using NetWork.Transport;

namespace Evil.Switcher
{
    public class LinkerNetWorkFactory : INetWorkFactory
    {
        public Session CreateSession(IChannelHandlerContext ctx)
        {
            return new LinkerSession(ctx);
        }

        public ISessionMgr CreateSessionMgr()
        {
            return new LinkerSessionMgr();
        }

        public IMessageRegister CreateMessageRegister()
        {
            return Switcher.I.MessageRegister;
        }

        public IMessageProcessor CreateMessageProcessor(TransportConfig config)
        {
            return new LinkerMessageProcessor();
        }
    }
}