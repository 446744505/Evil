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
        private readonly ConcurrentDictionary<long, Func<Stream, bool>> m_Pending = new();
        public void PendRequest(long requestId, Func<Stream, bool> func)
        {
            m_Pending[requestId] = func;
        }

        public Func<Stream, bool>? RemovePending(long requestId)
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