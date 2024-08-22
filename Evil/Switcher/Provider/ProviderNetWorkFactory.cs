using DotNetty.Transport.Channels;
using NetWork;

namespace Evil.Switcher
{
    public class ProviderNetWorkFactory : INetWorkFactory
    {
        public Session CreateSession(IChannelHandlerContext ctx)
        {
            return new ProviderSession(ctx);
        }

        public ISessionMgr CreateSessionMgr()
        {
            return new ProviderSessionMgr();
        }

        public IMessageRegister CreateMessageRegister()
        {
            return Switcher.I.MessageRegister;
        }

        public IMessageProcessor CreateMessageProcessor(ushort pvid)
        {
            return new ProviderMessageProcessor(pvid);
        }
    }
}