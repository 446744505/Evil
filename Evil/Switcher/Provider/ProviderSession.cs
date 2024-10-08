﻿using DotNetty.Transport.Channels;
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

        internal ProvideInfo? ProvideInfo { get; set; }
        internal ushort Pvid => (ushort)(ProvideInfo?.pvid ?? 0);
        internal bool IsAlive() => Time.Now - m_AliveTime < Provider.I.SessionTimeout;

        #endregion

        public ProviderSession(IChannelHandlerContext context) : base(context)
        {
        }

        public override Task SendAsync(Message msg)
        {
            if (msg.InnerPvid == 0)
            {
                var pvid = Pvid;
                if (pvid == 0)
                {
                    Log.I.Error($"not bind provide {this} msg {msg}");
                    return Task.CompletedTask;
                }
                msg.InnerPvid = pvid;
            }
            return base.SendAsync(msg);
        }

        public async Task Process(BindProvide bp)
        {
            ProvideInfo = bp.info;

            await Provider.I.Sessions.Bind(this);
            Log.I.Info($"bind provide {bp}");
        }
        
        public override string ToString()
        {
            return $"pvid:{Pvid} {base.ToString()}";
        }
    }
}