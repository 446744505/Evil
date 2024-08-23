using Evil.Provide;
using Evil.Util;

namespace Game.NetWork
{
    public class GameClientContext : ClientContext
    {
        public long PlayerId { get; }
        public GameClientContext(long clientSessionId, ProvideSession session, long playerId) : base(clientSessionId, session)
        {
            PlayerId = playerId;
        }

        public override void OnClientBroken()
        {
            Net.I.RemovePlayer(PlayerId);
            Log.I.Info($"client {ClientSessionId} broken");
        }
    }
}