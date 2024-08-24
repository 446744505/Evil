using DotNetty.Transport.Channels;
using NetWork;

namespace Evil.Provide
{
    public class ProvideNetWorkFactory : INetWorkFactory
    {
        private readonly IMessageRegister m_MessageRegister;

        public ProvideNetWorkFactory(IMessageRegister messageRegister)
        {
            m_MessageRegister = messageRegister;
        }

        public Session CreateSession(IChannelHandlerContext ctx)
        {
            return new ProvideSession(ctx);
        }

        public ISessionMgr CreateSessionMgr()
        {
            return new ProvideSessionMgr();
        }

        public IMessageRegister CreateMessageRegister()
        {
            return m_MessageRegister;
        }
    }
}