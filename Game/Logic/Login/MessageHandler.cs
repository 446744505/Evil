using System.Threading.Tasks;
using Evil.Provide;
using Game.NetWork;

namespace Proto
{
    public partial class LoginReq
    {
        public override async Task<bool> Process()
        {
            var provideSession = (ProvideSession)Session;
            var ctx = (ClientMsgBox)Context!;
            // 设置上下文
            var clientContext = new GameClientContext(ctx.clientSessionId, provideSession, playerId);
            provideSession.AddClient(clientContext);
            // 上线
            await Net.I.AddPlayer(playerId, clientContext);
            await clientContext.SendAsync(new LoginNtf());
            return true;
        }
    }
}