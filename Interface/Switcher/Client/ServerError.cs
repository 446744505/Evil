using Attributes;

namespace Evil.Switcher.Client
{
    [Protocol(Node.Switcher | Node.Client)]
    public class ServerError
    {
        public const int NotExistProvide = 9001;
        
        [ProtocolField(1)]
        private int pvid;
        [ProtocolField(2)]
        private int code;
    }
}