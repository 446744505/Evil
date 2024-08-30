using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace NetWork.Transport
{
    public interface ITransport : IDisposable
    {
        TransportConfig Config();
        RpcMgr RpcMgr();
        void Start();
    }
    
    public class RpcMgr
    {
        private readonly ConcurrentDictionary<long, Action<Stream>> m_Pending = new();
        public void PendRequest(long requestId, Action<Stream> cb)
        {
            m_Pending[requestId] = cb;
        }

        public Action<Stream>? RemovePending(long requestId)
        {
            m_Pending.TryRemove(requestId, out var func);
            return func;
        }

        public async Task DisposeAsync()
        {
            while (m_Pending.Count > 0)
            {
                await Task.Delay(1000);
            }
        }
    }
}