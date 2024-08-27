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
        public ITransport Transport { get; set; } = null!;
        public TransportConfig Config => Transport.Config();

        public Session(IChannelHandlerContext context)
        {
            Id = IdGenerator.NextId();
            m_Context = context;
        }
        
        public virtual Task SendAsync(Message msg)
        {
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