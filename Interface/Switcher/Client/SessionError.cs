using Attributes;

namespace Evil.Switcher.Client
{
    [Protocol(Node.Switcher|Node.Client)]
    public class SessionError
    {
        public const int PingTimeout = 8001; // 心跳超时
        public const int OverMaxSession = 8002; // 超过最大连接数
        public const int RateLimit = 8003; // 发送频率过快
        public const int CanNotToProvide = 8004; // 当前状态不能发送协议到provide
            
        [ProtocolField(1)]
        private int code;
    }
}