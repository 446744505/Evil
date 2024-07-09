using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NetWork
{
    public class AcceptorSessionMgr : ISessionMgr
    {
        private readonly ConcurrentDictionary<long, Session> m_Sessions = new();
        public virtual void OnAddSession(Session session)
        {
            m_Sessions.TryAdd(session.Id, session);
        }

        public virtual void OnRemoveSession(Session session)
        {
            m_Sessions.Remove(session.Id, out _);
        }
    }
}