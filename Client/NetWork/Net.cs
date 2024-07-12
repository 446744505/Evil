using System.Threading.Tasks;
using NetWork;
using NetWork.Util;

namespace Client.NetWork
{
    public class Net : Singleton<Net>
    {
        private readonly ConnectorSessionMgr m_SessionMgr = new();
        private readonly MessageRegister m_MessageRegister = new();
        public ConnectorSessionMgr SessionMgr => m_SessionMgr;
        public IMessageRegister MessageRegister => m_MessageRegister;

        public void Send(Message msg)
        {
            msg.Send(m_SessionMgr.Session);
        }

        public async Task<T> SendAsync<T>(Rpc<T> rpc)
        {
            return await rpc.SendAsync(m_SessionMgr.Session);
        }
    }
}