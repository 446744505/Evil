using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork;

namespace Evil.Switcher
{
    public class ProviderSession : StateSession
    {
        private long m_AliveTime = Time.Now;
        internal bool IsAlive() => Time.Now - m_AliveTime < Provider.I.SessionTimeout;
        
        public ProviderSession(IChannelHandlerContext context) : base(context)
        {
        }
    }
}