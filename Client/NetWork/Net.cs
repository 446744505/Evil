using System.Threading.Tasks;
using Client.Hero;
using Evil.Util;
using NetWork;

namespace Client.NetWork
{
    public class Net : Singleton<Net>
    {
        private readonly ConnectorSessionMgr m_SessionMgr = new ClientSessionMgr();
        private readonly MessageRegister m_MessageRegister = new();
        public ConnectorSessionMgr SessionMgr => m_SessionMgr;
        public IMessageRegister MessageRegister => m_MessageRegister;

        public void Send(Message msg)
        {
            msg.Send(m_SessionMgr.Session);
        }

        public async Task<T> SendAsync<T>(Rpc<T> rpc) where T : Message
        {
            return await rpc.SendAsync(m_SessionMgr.Session);
        }
    }

    public class ClientSessionMgr : ConnectorSessionMgr
    {
        public override void OnAddSession(Session session)
        {
            base.OnAddSession(session);
            Task.Run(() => HeroMgr.I.Test());
        }
    }
}