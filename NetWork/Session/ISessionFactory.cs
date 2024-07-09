using DotNetty.Transport.Channels;

namespace NetWork
{
    public interface ISessionFactory
    {
        public Session CreateSession(IChannelHandlerContext ctx);
        public ISessionMgr CreateSessionMgr();
    }
    
    public class DefaultAcceptorSessionFactory : ISessionFactory
    {
        public virtual Session CreateSession(IChannelHandlerContext ctx)
        {
            return new Session(ctx);
        }

        public virtual ISessionMgr CreateSessionMgr()
        {
            return new AcceptorSessionMgr();
        }
    }
    
    public class DefaultConnectorSessionFactory : ISessionFactory
    {
        public virtual Session CreateSession(IChannelHandlerContext ctx)
        {
            return new Session(ctx);
        }

        public virtual ISessionMgr CreateSessionMgr()
        {
            return new ConnectorSessionMgr();
        }
    }
}