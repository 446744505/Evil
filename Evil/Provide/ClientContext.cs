using NetWork;

namespace Evil.Provide
{
    public abstract class ClientContext
    {
        private readonly long m_ClientSessionId;
        private readonly ProvideSession m_Session;
        
        public long ClientSessionId => m_ClientSessionId;
        public ProvideSession Session => m_Session;

        protected ClientContext(long clientSessionId, ProvideSession session)
        {
            m_ClientSessionId = clientSessionId;
            m_Session = session;
        }

        /// <summary>
        /// 发送消息到对应的客户端
        /// </summary>
        /// <param name="msg"></param>
        public async Task SendAsync(Message msg)
        {
            await m_Session.SendToClientAsync(m_ClientSessionId, msg);
        }

        public abstract void OnClientBroken();
    }
}