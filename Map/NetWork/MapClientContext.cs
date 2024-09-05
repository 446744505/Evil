using Evil.Provide;
using Evil.Util;

namespace Map.NetWork
{
    public class MapClientContext : ClientContext
    {
        public long PlayerId { get; }
        public MapClientContext(long clientSessionId, ProvideSession session, long playerId) : base(clientSessionId, session)
        {
            PlayerId = playerId;
        }

        public override void OnClientBroken()
        {
            Log.I.Info($"client {ClientSessionId} broken");
        }
    }
}