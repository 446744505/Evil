
using NetWork;

namespace Evil.Provide
{
    public partial class Provide
    {
        public void SendToProvide(ushort pvid, Message msg)
        {
            var session = m_Sessions.FindProvideSession(pvid);
            msg.Pvid = pvid;
            msg.Send(session);
        }

        public async Task<T?> SendToProvideAsync<T>(ushort pvid, Rpc<T> rpc) where T : Message
        {
            var session = m_Sessions.FindProvideSession(pvid);
            rpc.Pvid = pvid;
            return await rpc.SendAsync(session);
        }
        
        /// <summary>
        /// 某个provider是否连接了某个provide
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="pvid"></param>
        /// <returns></returns>
        public bool IsProviderLinkProvide(string provider, ushort pvid)
        {
            if (!m_Provides.TryGetValue(provider, out var infos))
            {
                return false;
            }

            return infos.ContainsKey(pvid);
        }
    }
}