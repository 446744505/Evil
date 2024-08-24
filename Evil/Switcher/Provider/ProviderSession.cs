using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork;
using Proto;

namespace Evil.Switcher
{
    public class ProviderSession : StateSession
    {
        #region 字段

        private long m_AliveTime = Time.Now;

        #endregion

        #region 属性

        internal ushort Pvid;
        internal bool IsAlive() => Time.Now - m_AliveTime < Provider.I.SessionTimeout;

        #endregion

        public ProviderSession(IChannelHandlerContext context) : base(context)
        {
        }

        public override Task SendAsync(Message msg)
        {
            if (msg.Pvid == 0)
            {
                if (Pvid == 0)
                {
                    Log.I.Error($"not bind provide {this} msg {msg}");
                    return Task.CompletedTask;
                }
                msg.Pvid = Pvid;
            }
            return base.SendAsync(msg);
        }

        public async Task Process(BindProvide bp)
        {
            Pvid = (ushort)bp.pvid;
            
            await Provider.I.Sessions.Bind(this);
            Log.I.Info($"bind provide {bp}");
        }
        
        public override string ToString()
        {
            return $"pvid {Pvid} {base.ToString()}";
        }
    }
}