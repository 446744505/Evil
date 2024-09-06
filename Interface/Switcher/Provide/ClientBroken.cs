using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher, 0)]
    public class ClientBroken
    {
        [ProtocolField(1)]
        private long clientSessionId;
    }
}