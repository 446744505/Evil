using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Evil.Util;

namespace NetWork
{
    public class Session
    {
        private readonly IChannelHandlerContext m_Context;
        public long Id { get; }

        public Session(IChannelHandlerContext context)
        {
            Id = IdGenerator.NextId();
            m_Context = context;
        }
        
        public Task Send(Message msg)
        {
            return m_Context.WriteAndFlushAsync(msg);
        }

        public virtual void OnClose()
        {
        }

        public override string ToString()
        {
            return $"{Id} {m_Context.Channel.RemoteAddress}";
        }
    }
}