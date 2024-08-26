using System;
using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork.Transport;

namespace NetWork.Handler
{
    internal class LogicHandler : SimpleChannelInboundHandler<Message>
    {
        private Session m_Session = null!;
        private readonly TransportConfig m_Config;
        private readonly ISessionMgr m_SessionMgr;

        public LogicHandler(TransportConfig config, ISessionMgr sessionMgr)
        {
            m_Config = config;
            m_SessionMgr = sessionMgr;
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            base.ChannelActive(ctx);
            m_Session = m_Config.NetWorkFactory!.CreateSession(ctx);
            ctx.GetAttribute(AttrKey.Session).Set(m_Session);
            m_Session.Config = m_Config;
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
            Log.I.Debug($"receive {msg} session {m_Session}");
            msg.Dispatch();
        }
    }
}