using NetWork;

namespace Client.NetWork
{
    public class ClientSessionFactory : DefaultConnectorSessionFactory
    {
        public override ISessionMgr CreateSessionMgr()
        {
            return new ClientSessionMgr();
        }
    }
    
    public class ClientSessionMgr : ConnectorSessionMgr
    {
        public override void OnAddSession(Session session)
        {
            base.OnAddSession(session);
        }
    }
}