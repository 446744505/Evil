using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork.Transport;

namespace NetWork
{
    public class Session
    {
        private readonly IChannelHandlerContext m_Context;
        public long Id { get; }
        public TransportConfig Config { get; set; } = null!;

        public Session(IChannelHandlerContext context)
        {
            Id = IdGenerator.NextId();
            m_Context = context;
        }
        
        public virtual Task SendAsync(Message msg)
        {
            Log.I.Debug($"send {msg} session {this}");
            return m_Context.WriteAndFlushAsync(msg);
        }

        public virtual void OnClose()
        {
        }

        public async Task CloseAsync()
        {
            await m_Context.CloseAsync();
        }

        public override string ToString()
        {
            return $"sid:{Id} {m_Context.Channel.RemoteAddress}";
        }
    }
}