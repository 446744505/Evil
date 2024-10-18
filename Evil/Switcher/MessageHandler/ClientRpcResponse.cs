using Evil.Switcher;
using NetWork.Proto;

namespace Proto
{
    public partial class ClientRpcResponse
    {
        public override void Dispatch()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession is not null)
            {
                linkerSession.Send(new RpcResponse
                {
                    RequestId = requestId,
                    Data = data,
                });
            }
        }
    }
}