using NetWork;

namespace Client.NetWork
{
    public class ClientSessionFactory : DefaultConnectorSessionFactory
    {
        public override ISessionMgr CreateSessionMgr()
        {
            return Net.I.SessionMgr;
        }
    }
    
    public class ClientSessionMgr : ConnectorSessionMgr
    {
        public override void OnAddSession(Session session)
        {
            base.OnAddSession(session);
            // Net.I.Send();
        }
    }
}