
using Evil.Util;
using Proto;

namespace Evil.Provide
{
    public class ProvideSessions
    {
        private readonly Provide m_Provide;
        private readonly LockAsync m_Lock = new();
        private readonly List<ProvideSession> m_Sessions = new();
        /// <summary>
        /// 记录了当前节点到每个pvid发送消息的通道，用于保证消息顺序
        /// </summary>
        private readonly Dictionary<ushort, ProvideSession> m_Dispatcher = new();

        public ProvideSessions(Provide provide)
        {
            m_Provide = provide;
        }

        public void Add(ProvideSession session)
        {
            var release = m_Lock.WLock();
            try
            {
                m_Sessions.Add(session);
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        public void Remove(ProvideSession session)
        {
            var release = m_Lock.WLock();
            try
            {
                m_Sessions.Remove(session);
                // 自己与provider断开的修复
                TryFixDispatcher(session.ProviderUrl);
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        /// <summary>
        /// 连接某个provider的provide有变化
        /// </summary>
        /// <param name="providerUrl"></param>
        /// <param name="newAll"></param>
        internal void OnProvideUpdate(
            string providerUrl,
            Dictionary<ushort, ProvideInfo> newAll, 
            List<ProvideInfo> added, 
            List<ProvideInfo> removed)
        {
            List<ushort> willRemove = new();
            var release = m_Lock.RLock();
            try
            {
                // 看看是否有已经建立的通道被影响了
                foreach (var pair in m_Dispatcher)
                {
                    // 不是通过这个provider联通，不用管
                    if (pair.Value.ProviderUrl != providerUrl)
                        continue;
                    // 新的所有provide里不存在当前dispatch的pvid了
                    if (!newAll.ContainsKey(pair.Key))
                    {
                        willRemove.Add(pair.Key);
                    }
                }
            }
            finally
            {
                m_Lock.RUnlock(release);
            }

            if (willRemove.Count > 0)
            {
                release = m_Lock.WLock();
                try
                {
                    foreach (var pvid in willRemove)
                    {
                        m_Dispatcher.Remove(pvid);
                        Log.I.Info($"remote broken remove provide {pvid} dispatcher for provider {providerUrl}");
                    }
                }
                finally
                {
                    m_Lock.WUnlock(release);
                }
            }
        }

        /// <summary>
        /// 需要处于锁保护中
        /// 与某个provider断开后，要找出所有用该provider发送消息的通道，并删除
        /// 等重新发消息的时候再找一个，不在这里提前找
        /// (现实情况大概率是跟所有provider都断了，如果在这里找会导致瞬间发生A断-找到B-B断-找到C-...的情况)
        /// </summary>
        private void TryFixDispatcher(string providerUrl)
        {
            List<ushort> willRemove = new();
            foreach (var pair in m_Dispatcher)
            {
                if (pair.Value.ProviderUrl == providerUrl)
                {
                    willRemove.Add(pair.Key);
                }
            }
            foreach (var pvid in willRemove)
            {
                m_Dispatcher.Remove(pvid);
                Log.I.Info($"local broken,remove provide {pvid} dispatcher for provider {providerUrl}");
            }
        }

        /// <summary>
        /// 找到一个可以联通到toPvid的session
        /// </summary>
        /// <param name="toPvid"></param>
        /// <returns></returns>
        internal ProvideSession? FindProvideSession(ushort toPvid)
        {
            ProvideSession[] shuffle;
            var findIdx = -1;
            var release = m_Lock.RLock();
            try
            {
                if (m_Dispatcher.TryGetValue(toPvid, out var find))
                {
                    return find;
                }
                // 这里的session肯定是我连上的
                shuffle = m_Sessions.ToArray();
                // 打乱
                Edb.Edb.I.Random.Shuffle(shuffle);
                // 从第一个开始找，找到就返回
                for (var i = 0; i < shuffle.Length; i++)
                {
                    var session = shuffle[i];
                    if (m_Provide.IsSelfLinkProvide(session.ProviderUrl, toPvid))
                    {
                        findIdx = i;
                        break;
                    }
                }
            }
            finally
            {
                m_Lock.RUnlock(release);
            }

            if (findIdx > -1)
            {
                release = m_Lock.WLock();
                try
                {
                    var find = shuffle[findIdx];
                    m_Dispatcher[toPvid] = find;
                    Log.I.Info($"add provide {toPvid} dispatcher at provider {find.ProviderUrl}");
                    return find;
                }
                finally
                {
                    m_Lock.WUnlock(release);
                }
            }
            
            return null;
        }
    }
}