using System.Threading.Tasks;
using Evil.Util;
using NetWork;

namespace Game.NetWork
{
    public partial class Net
    {
        public void SendToPlayerWhenCommit(long playerId, Message msg)
        {
            ProcedureHelper.ExecuteWhenCommit(() => SendToPlayer(playerId, msg));
        }
        
        public void SendToPlayer(long playerId, Message msg)
        {
            if (!m_Players.TryGetValue(playerId, out var ctx))
            {
                return;
            }

            ctx.Send(msg);
        }
    }
}