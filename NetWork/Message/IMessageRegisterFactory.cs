
namespace NetWork
{
    public interface IMessageRegisterFactory
    {
        public IMessageRegister CreateMessageRegister();

        public IMessageProcessor CreateMessageProcessor(ushort pvid)
        {
            return new MessageProcessor(pvid);
        }
    }
}