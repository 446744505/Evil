using DotNetty.Transport.Channels;
using NetWork;
using NetWork.Proto;
using NetWork.Transport;
using Proto;

namespace Evil.Provide
{
    public class ProvideNetWorkFactory : INetWorkFactory
    {
        private readonly IMessageRegister m_MessageRegister;

        public ProvideNetWorkFactory(IMessageRegister messageRegister)
        {
            m_MessageRegister = messageRegister;
        }

        public Session CreateSession(IChannelHandlerContext ctx)
        {
            return new ProvideSession(ctx);
        }

        public ISessionMgr CreateSessionMgr()
        {
            return new ProvideSessionMgr();
        }

        public IMessageRegister CreateMessageRegister()
        {
            return m_MessageRegister;
        }

        public Message? CreateRpcResponse(object? ctx, long requestId, byte[] data)
        {
            // 是客户端来的rpc
            if (ctx is ClientMsgBox box)
            {
                return new ClientRspResponse
                {
                    requestId = requestId,
                    clientSessionId = box.clientSessionId,
                    data = data,
                };
            }

            return null;
        }
        
        public IMessageProcessor CreateMessageProcessor(TransportConfig config)
        {
            var processor = new MessageProcessor();
            processor.Pvid = ((ProvideConnectorTransportConfig)config).Provide.Pvid;
            return processor;
        }
    }
}