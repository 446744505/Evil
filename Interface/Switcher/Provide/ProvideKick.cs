using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher)]
    public class ProvideKick
    {
        public const int Exception = 1001; // 收到客户端消息Dispatch时异常
        public const int Offline = 1002; // 下线
        public const int HadLogin = 1003; // 已经登录
        public const int NotMap = 1004; // 没有map可用
        
        [ProtocolField(1)]
        private long clientSessionId;
        [ProtocolField(2)]
        private int code;
    }
}