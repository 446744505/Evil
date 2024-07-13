using System;
using System.Collections.Concurrent;
using System.IO;
using NetWork.Util;

namespace NetWork
{
    public class RpcMgr : Singleton<RpcMgr>
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
    }
}