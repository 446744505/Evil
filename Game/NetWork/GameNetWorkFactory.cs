using DotNetty.Transport.Channels;
using NetWork;

namespace Game.NetWork;

public class GameNetWorkFactory : INetWorkFactory
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