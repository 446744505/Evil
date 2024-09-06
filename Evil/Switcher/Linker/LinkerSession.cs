using System.Collections.Immutable;
using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork;

namespace Evil.Switcher
{
    internal class LinkerSession : Session
    {
        private long m_AliveTime = Time.Now;
        private ImmutableHashSet<ushort> m_BindProvides = ImmutableHashSet.Create<ushort>();
        internal bool IsAlive() => Time.Now - m_AliveTime < Linker.I.SessionTimeout;
        
        internal ISet<ushort> BindProvides => m_BindProvides;
        
        internal LinkerSession(IChannelHandlerContext context) : base(context)
        {
        }

        internal void BindProvide(ushort pvid)
        {
            m_BindProvides = m_BindProvides.Add(pvid);
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