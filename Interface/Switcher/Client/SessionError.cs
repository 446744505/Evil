using Attributes;

namespace Evil.Switcher.Client
{
    [Protocol(Node.Switcher|Node.Client)]
    public class SessionError
    {
        public const int PingTimeout = 1; // 心跳超时
        public const int OverMaxSession = 2; // 超过最大连接数
        public const int RateLimit = 3; // 发送频率过快
        public const int CanNotToProvide = 4; // 当前状态不能发送协议到provide
            
        [ProtocolField(1)]
        private int code;
    }
}