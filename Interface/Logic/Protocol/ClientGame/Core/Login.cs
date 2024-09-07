using Attributes;

namespace Login
{
    [Protocol(Node.Client|Node.Game)]
    public class LoginReq
    {
        [ProtocolField(1)]
        private string account;
    }

    [Protocol(Node.Client|Node.Game)]
    public class LoginNtf
    {
        [ProtocolField(1)]
        private long playerId;
        [ProtocolField(2)]
        private int mapPvid;
    }
}