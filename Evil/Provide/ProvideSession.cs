using DotNetty.Transport.Channels;
using NetWork;

namespace Evil.Provide
{
    public class ProvideSession : Session
    {
        public ProvideSession(IChannelHandlerContext context) : base(context)
        {
        }
    }
}