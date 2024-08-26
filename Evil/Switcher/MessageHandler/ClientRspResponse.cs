using Evil.Switcher;
using NetWork.Proto;

namespace Proto
{
    public partial class ClientRspResponse
    {
        public override async Task Dispatch()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession == null)
            {
                // TODO 与SendToClient一样处理
            }
            else
            {
                await linkerSession.SendAsync(new RpcResponse
                {
                    RequestId = requestId,
                    Data = data,
                });
            }
        }
    }
}