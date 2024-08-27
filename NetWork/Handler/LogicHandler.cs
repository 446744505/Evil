using System;
using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork.Transport;

namespace NetWork.Handler
{
    internal class LogicHandler : SimpleChannelInboundHandler<Message>
    {
        private Session m_Session = null!;
        private readonly ITransport m_Transport;
        private readonly ISessionMgr m_SessionMgr;

        public LogicHandler(ITransport transport, ISessionMgr sessionMgr)
        {
            m_Transport = transport;
            m_SessionMgr = sessionMgr;
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            base.ChannelActive(ctx);
            var config = m_Transport.Config();
            m_Session = config.NetWorkFactory!.CreateSession(ctx);
            ctx.GetAttribute(AttrKey.Session).Set(m_Session);
            m_Session.Transport = m_Transport;
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
            Log.I.Error($"LogicHandler.ExceptionCaught, session {m_Session}", exception);
            context.CloseAsync();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            msg.Dispatch();
        }
    }
}