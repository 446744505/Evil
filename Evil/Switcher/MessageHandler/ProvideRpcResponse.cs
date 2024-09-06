using Evil.Switcher;
using Evil.Util;
using NetWork.Proto;

namespace Proto
{
    public partial class ProvideRpcResponse
    {
        public override async Task Dispatch()
        {
            var providerSession = Provider.I.Sessions.GetSession((ushort)pvid);
            if (providerSession is null)
            {
                Log.I.Error($"provide {pvid} rpc response fail, session is null");
                return;
            }

            await providerSession.SendAsync(new RpcResponse
            {
                RequestId = requestId,
                Data = data,
            });
        }
    }
}