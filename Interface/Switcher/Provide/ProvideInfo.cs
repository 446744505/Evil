using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher)]
    public class ProvideInfo
    {
        [ProtocolField(1)]
        private int pvid;
        [ProtocolField(2)]
        private int type; // 服务类型
    }
}