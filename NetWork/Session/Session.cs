using DotNetty.Transport.Channels;

namespace NetWork
{
    public class Session
    {
        private readonly IChannelHandlerContext m_Context;

        public Session(IChannelHandlerContext context)
        {
            m_Context = context;
        }

        public override string? ToString()
        {
            return m_Context.Channel.RemoteAddress.ToString();
        }
    }
}