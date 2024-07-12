using DotNetty.Transport.Channels;
using NetWork;

namespace Client.NetWork
{
    public class ClientNetWorkFactory : INetWorkFactory
    {
        public Session CreateSession(IChannelHandlerContext ctx)
        {
            return new Session(ctx);
        }

        public ISessionMgr CreateSessionMgr()
        {
            return Net.I.SessionMgr;
        }

        public IMessageRegister CreateMessageRegister()
        {
            return Net.I.MessageRegister;
        }
    }
}