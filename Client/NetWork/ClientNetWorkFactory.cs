using DotNetty.Transport.Channels;
using NetWork;
using Proto;

namespace Client.NetWork
{
    public class ClientNetWorkFactory : INetWorkFactory
    {
        private readonly MessageRegister m_MessageRegister = new();
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
            return m_MessageRegister;
        }
    }
}