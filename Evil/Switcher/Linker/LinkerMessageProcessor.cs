using NetWork;

namespace Evil.Switcher
{
    public class LinkerMessageProcessor : MessageProcessor
    {
        public LinkerMessageProcessor(ushort pvid) : base(pvid)
        {
        }

        protected override Message? WhenNotType(MessageHeader header, int readSize, BinaryReader reader)
        {
            return null;
        }
    }
}