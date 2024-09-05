
using System.Collections.Concurrent;
using Evil.Provide;
using NetWork;
using Proto;

namespace Map.NetWork
{
    public partial class Net
    {
        private readonly ConcurrentDictionary<long, MapClientContext> m_Players = new();
        
        public long PlayerId(Message clientMsg)
        {
            var ctx = (ClientMsgBox)clientMsg.Context!;
            var provideSession = (ProvideSession)clientMsg.Session;
            var clientContext = provideSession.GetClientContext(ctx.clientSessionId);
            if (clientContext == null)
            {
                throw new NullReferenceException($"client {ctx.clientSessionId} ctx is null");
            }
            return ((MapClientContext)clientContext).PlayerId;
        }
        
        
    }
}