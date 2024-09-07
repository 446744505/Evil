using System.Threading.Tasks;
using Edb;
using Evil.Provide;
using Evil.Util;
using Game;
using Game.Logic.Login;
using Game.NetWork;
using Login;
using NetWork;

namespace Proto
{
    public partial class LoginReq
    {
        public override async Task<bool> Process()
        {
            var provideSession = (ProvideSession)Session;
            var ctx = (ClientMsgBox)Context!;
            // 找一个地图
            var mapPvid = Program.Ctx.RandomMapPvid();
            if (mapPvid == 0)
            {
                await Session.SendAsync(new ProvideKick
                {
                    clientSessionId = ctx.clientSessionId,
                    code = ProvideKick.NotMap,
                });
                return false;
            }
            
            // 获取or创建角色
            var getOrCreate = new PGetOrCreatePlayer(account);
            if (!(await Procedure.Call(getOrCreate)).IsSuccess)
            {
                return false;
            }

            var playerId = getOrCreate.R;
            
            // 设置上下文
            var clientContext = new GameClientContext(ctx.clientSessionId, provideSession, playerId);
            provideSession.AddClient(clientContext);
            // 上线
            if (!await Net.I.AddPlayer(playerId, clientContext))
            {
                return false;
            }

            var providerUrl = provideSession.ProviderUrl;
            var loginMapAck = await LoginMgr.I.LoginService.LoginToMap(mapPvid, playerId, providerUrl, ctx.clientSessionId);
            if (loginMapAck.Code != RpcAck.Success)
            {
                Log.I.Error($"player {playerId} login to map failed {loginMapAck.Code}");
                return false;
            }
            Log.I.Info($"player {playerId} login to map {loginMapAck.data}");

            await Net.I.SendToPlayer(playerId,new LoginNtf{mapPvid = loginMapAck.data});
            return true;
        }
    }
}