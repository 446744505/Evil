
using NetWork.Transport;

namespace NetWork
{
    public interface IMessageRegisterFactory
    {
        public IMessageRegister CreateMessageRegister();

        public Message? CreateRpcResponse(object? ctx, long requestId, byte[] data)
        {
            return null;
        }
        
        public IMessageProcessor CreateMessageProcessor(TransportConfig config)
        {
            return new MessageProcessor();
        }
    }
}