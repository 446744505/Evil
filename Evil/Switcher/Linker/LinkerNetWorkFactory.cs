using DotNetty.Transport.Channels;
using NetWork;

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
    }
}