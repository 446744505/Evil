using System.Threading.Tasks;
using Evil.Util;
using NetWork;
using Proto;

namespace Client.NetWork
{
    public class Net : Singleton<Net>
    {
        private readonly ConnectorSessionMgr m_SessionMgr = new ClientSessionMgr();

        public ConnectorSessionMgr SessionMgr => m_SessionMgr;

        public ushort Pvid { get; set; } = 1;
        
        public void Send(Message msg)
        {
            msg.InnerPvid = Pvid;
            msg.Send(m_SessionMgr.Session);
        }

        public async Task<T> SendAsync<T>(Rpc<T> rpc) where T : RpcAck
        {
            rpc.InnerPvid = Pvid;
            return await rpc.SendAsync(m_SessionMgr.Session);
        }
    }

    public class ClientSessionMgr : ConnectorSessionMgr
    {
        public override void OnAddSession(Session session)
        {
            base.OnAddSession(session);
            
            // login
            Net.I.Send(new LoginReq { playerId = 99 });
        }
    }
}