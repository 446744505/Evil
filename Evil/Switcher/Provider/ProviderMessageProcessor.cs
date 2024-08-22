using NetWork;

namespace Evil.Switcher
{
    public class ProviderMessageProcessor : MessageProcessor
    {
        public ProviderMessageProcessor(ushort pvid) : base(pvid)
        {
        }

        protected override Message? WhenNotType(MessageHeader header, int readSize, BinaryReader reader)
        {
            return null;
        }
    }
}