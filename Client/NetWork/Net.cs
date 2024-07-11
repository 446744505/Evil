using System.Threading.Tasks;
using NetWork;
using NetWork.Util;

namespace Client.NetWork
{
    public class Net : Singleton<Net>
    {
        private readonly ConnectorSessionMgr m_SessionMgr = new ClientSessionMgr();
        public ConnectorSessionMgr SessionMgr => m_SessionMgr;

        public void Send(Message msg)
        {
            var session = m_SessionMgr.Session;
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }
            session.Send(msg);
        }

        public async Task<T> SendAsync<T>(Message msg)
        {
            var session = m_SessionMgr.Session;
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }
            return await session.SendAsync<T>(msg);
        }
    }
}