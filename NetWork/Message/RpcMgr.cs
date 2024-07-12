using System.Collections.Concurrent;
using System.Threading.Tasks;
using NetWork.Util;

namespace NetWork
{
    public class RpcMgr : Singleton<RpcMgr>
    {
        private readonly ConcurrentDictionary<long, TaskCompletionSource<Message>> m_Pending = new();

        public void PendRequest<T>(long requestId, TaskCompletionSource<T> completionSource) where T : Message
        {
            m_Pending[requestId] = completionSource;
        }
        
        public TaskCompletionSource<Message>? RemovePending(long requestId)
        {
            m_Pending.TryRemove(requestId, out var completionSource);
            return completionSource;
        }
    }
}