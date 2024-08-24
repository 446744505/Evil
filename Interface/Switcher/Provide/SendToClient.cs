using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher)]
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