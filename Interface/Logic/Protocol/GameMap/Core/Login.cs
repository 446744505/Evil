using Attributes;

namespace Login
{
    [Service(Node.Game, Node.Map)]
    public interface LoginService
    {
        public int LoginToMap(
            [ProtocolField(1)]long playerId, 
            [ProtocolField(2)]string providerUrl, 
            [ProtocolField(3)]long clientSessionId);
    }
}