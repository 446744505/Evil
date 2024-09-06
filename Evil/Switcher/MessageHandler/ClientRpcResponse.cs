using Evil.Switcher;
using NetWork.Proto;

namespace Proto
{
    public partial class ClientRpcResponse
    {
        public override async Task Dispatch()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession is not null)
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