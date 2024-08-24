using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork;

namespace Evil.Switcher
{
    internal class LinkerSession : Session
    {
        private long m_AliveTime = Time.Now;
        internal bool IsAlive() => Time.Now - m_AliveTime < Linker.I.SessionTimeout;
        
        internal LinkerSession(IChannelHandlerContext context) : base(context)
        {
        }

        internal void ResetAlive()
        {
            m_AliveTime = Time.Now;
        }

        public void ReceiveUnknown()
        {
            ResetAlive();
        }
    }
}