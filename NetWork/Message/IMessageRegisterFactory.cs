
namespace NetWork
{
    public interface IMessageRegisterFactory
    {
        public IMessageRegister CreateMessageRegister();

        public Message? CreateRpcResponse(object? ctx, long requestId, byte[] data)
        {
            return null;
        }
        
        public IMessageProcessor CreateMessageProcessor(ushort pvid)
        {
            return new MessageProcessor(pvid);
        }
    }
}