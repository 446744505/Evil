using System.Threading.Tasks;
using Evil.Util;
using NetWork;

namespace Game.NetWork
{
    public partial class Net
    {
        public void SendToPlayerWhenCommit(long playerId, Message msg)
        {
            ProcedureHelper.ExecuteWhenCommit(async () => await SendToPlayer(playerId, msg));
        }
        
        public async Task SendToPlayer(long playerId, Message msg)
        {
            if (!m_Players.TryGetValue(playerId, out var ctx))
            {
                return;
            }

            await ctx.SendAsync(msg);
        }
    }
}