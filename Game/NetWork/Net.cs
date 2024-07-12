using System.Threading.Tasks;
using Logic.Hero.Proto;
using NetWork;
using NetWork.Util;

namespace Game.NetWork
{
    public class Net : Singleton<Net>
    {
        private readonly AcceptorSessionMgr m_SessionMgr = new();
        private readonly MessageRegister m_MessageRegister = new();
        public AcceptorSessionMgr SessionMgr => m_SessionMgr;
        public IMessageRegister MessageRegister => m_MessageRegister;

        public void Send(Message msg)
        {
            var session = m_SessionMgr.GetSession(1);
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }
            session.Send(msg);
        }

        public async Task<T> SendAsync<T>(Message msg)
        {
            var session = m_SessionMgr.GetSession(1);
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }
            return await session.SendAsync<T>(msg);
        }
    }
}