using Attributes;

namespace Login
{
    [Service(Node.Game, Node.Map)]
    public interface LoginService
    {
        public bool LoginToMap(
            [ProtocolField(1)]long playerId, 
            [ProtocolField(2)]string providerUrl, 
            [ProtocolField(3)]long clientSessionId);
    }
}