using Attributes;

namespace Login
{
    [Protocol(Node.Client|Node.Game)]
    public class LoginReq
    {
        [ProtocolField(1)]
        private long playerId;
    }

    [Protocol(Node.Client|Node.Game)]
    public class LoginNtf
    {
        [ProtocolField(1)]
        private int mapPvid;
    }
}