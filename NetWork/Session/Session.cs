using System.Net;
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

        public string RemoteAddress()
        {
            var endPoint = m_Context.Channel.RemoteAddress;
            var ipEndPoint = (IPEndPoint)endPoint;
            return $"{ipEndPoint.Address.MapToIPv4().ToString()}:{ipEndPoint.Port}";
        }
        
        public virtual void Send(Message msg)
        {
            m_Context.WriteAndFlushAsync(msg).Wait();
        }

        public virtual void OnClose()
        {
        }

        public void Close()
        {
            m_Context.CloseAsync().Wait();
        }

        public override string ToString()
        {
            return $"sid:{Id} {m_Context.Channel.RemoteAddress}";
        }
    }
}