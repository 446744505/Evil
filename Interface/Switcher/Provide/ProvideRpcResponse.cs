using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher, 0)]
    public class ProvideRpcResponse
    {
        [ProtocolField(1)]
        private long requestId;
        [ProtocolField(2)]
        private int pvid;
        [ProtocolField(3)]
        private byte[] data;
    }
}