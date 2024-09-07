using System.Threading.Tasks;
using Edb;
using Evil.Util;
using XBean;

namespace Game.Logic.Login;

public class PLogicOnline : Procedure
{
    private readonly long m_PlayerId;

    public PLogicOnline(long playerId)
    {
        m_PlayerId = playerId;
    }

    public async Task<bool> Process()
    {
        var p = await XTable.Player.Update(m_PlayerId);
        if (p is null)
        {
            p = new Player()
            {
                PlayerId = m_PlayerId,
                ServerId = CmdLine.I.ServerId,
                Level = 1,
                PlayerName = $"Player{m_PlayerId}",
            };
            await XTable.Player.Insert(p);
            // 将player加入server映射表
            ProcedureHelper.ExecuteWhenCommit(new PAddPlayerToServer(p.ServerId, p.PlayerId));
            Log.I.Info($"create player {m_PlayerId}");
        }
        Log.I.Info($"player {m_PlayerId} logic online");
        return true;
    }
}