using System.Threading.Tasks;
using Edb;
using Evil.Util;
using XBean;

namespace Game.Logic.Login
{
    public class PAddPlayerToServer : Procedure
    {
        private readonly int m_ServerId;
        private readonly long m_PlayerId;

        public PAddPlayerToServer(int serverId, long playerId)
        {
            m_ServerId = serverId;
            m_PlayerId = playerId;
        }

        public async Task<bool> Process()
        {
            var server = await XTable.Server.Update(m_ServerId);
            if (server is null)
            {
                server = new Server();
                server.ServerId = m_ServerId;
                await XTable.Server.Insert(server);
            }

            server.PlayerIds.Add(m_PlayerId);
            Log.I.Info($"server {m_ServerId} add player {m_PlayerId}");
            return true;
        }
    }
}