using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher)]
    public class ClientMsgBox
    {
        [ProtocolField(1)]
        private long clientSessionId;
        [ProtocolField(2)]
        private uint messageId;
        [ProtocolField(3)]
        private int pvid;
        [ProtocolField(4)]
        private byte[] data;
    }
}