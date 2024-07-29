using System.Threading.Tasks;
using Evil.Util;
using NetWork;
using Proto;

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
            msg.Send(session);
        }

        public async Task<T> SendAsync<T>(Rpc<T> rpc) where T : Message
        {
            var session = m_SessionMgr.GetSession(1);
            return await rpc.SendAsync(session);
        }
    }
}