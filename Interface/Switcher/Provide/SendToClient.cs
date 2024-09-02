using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher, 0)]
    public class SendToClient
    {
        [ProtocolField(1)]
        private long clientSessionId;
        [ProtocolField(2)]
        private uint messageId;
        [ProtocolField(3)]
        private byte[] data;
    }
}