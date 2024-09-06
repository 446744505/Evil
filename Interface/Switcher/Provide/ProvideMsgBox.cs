using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher, 0)]
    public class ProvideMsgBox
    {
        [ProtocolField(1)]
        private uint messageId;
        [ProtocolField(2)]
        private byte[] data;
        [ProtocolField(3)]
        private int fromPvid;
    }
}