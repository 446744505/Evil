
using NetWork;

namespace Evil.Provide
{
    public partial class Provide
    {
        public void SendToProvide(ushort pvid, Message msg)
        {
            var session = m_Sessions.FindProvideSession(pvid);
            msg.InnerPvid = pvid;
            msg.Send(session);
        }

        public async Task<T?> SendToProvideAsync<T>(ushort pvid, Rpc<T> rpc) where T : RpcAck
        {
            var session = m_Sessions.FindProvideSession(pvid);
            rpc.InnerPvid = pvid;
            return await rpc.SendAsync(session);
        }
        
        /// <summary>
        /// 自己是否通过某个provider能连通某个provide
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="pvid"></param>
        /// <returns></returns>
        public bool IsSelfLinkProvide(string provider, ushort pvid)
        {
            if (!m_Provides.TryGetValue(provider, out var infos))
            {
                return false;
            }

            // 即有对端也有自己
            return infos.ContainsKey(pvid) && infos.ContainsKey(Pvid);
        }
    }
}