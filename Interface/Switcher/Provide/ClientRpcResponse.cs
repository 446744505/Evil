using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher, 0)]
    public class ClientRpcResponse
    {
        [ProtocolField(1)]
        private long requestId;
        [ProtocolField(2)]
        private long clientSessionId;
        [ProtocolField(3)]
        private byte[] data;
    }
}