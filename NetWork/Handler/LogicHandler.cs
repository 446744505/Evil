using System;
using DotNetty.Transport.Channels;

namespace NetWork.Handler
{
    internal class LogicHandler : SimpleChannelInboundHandler<Message>
    {
        private Session m_Session;
        private readonly ISessionMgr m_SessionMgr;
        private readonly ISessionFactory m_SessionFactory;

        public LogicHandler(ISessionFactory sessionFactory, ISessionMgr sessionMgr)
        {
            m_SessionFactory = sessionFactory;
            m_SessionMgr = sessionMgr;
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            
            m_Session = m_SessionFactory.CreateSession(ctx);
            base.ChannelActive(ctx);
            m_SessionMgr.OnAddSession(m_Session);
            Log.I.Info($"add session {m_Session}");
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            m_SessionMgr.OnRemoveSession(m_Session);
            Log.I.Info($"remove session {m_Session}");
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
            Log.I.Error(exception);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            msg.Session = m_Session;
            Console.WriteLine(msg);
        }
    }
}