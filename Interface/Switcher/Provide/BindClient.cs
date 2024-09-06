using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher)]
    public class BindClient
    {
        [ProtocolField(1)]
        private long clientSessionId;
    }
}