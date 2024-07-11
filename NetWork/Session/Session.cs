using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using NetWork.Util;

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
        
        public void Send(Message msg)
        {
            m_Context.WriteAsync(msg);
        }

        public async Task<T> SendAsync<T>(Message msg)
        {
            await m_Context.WriteAsync(msg);
            var taskCompletionSource = new TaskCompletionSource<T>();
            return await taskCompletionSource.Task;
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