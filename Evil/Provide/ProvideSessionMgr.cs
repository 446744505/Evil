using NetWork;
using Proto;

namespace Evil.Provide;

public class ProvideSessionMgr : ConnectorSessionMgr
{
    public override void OnAddSession(Session session)
    {
        base.OnAddSession(session);
        
        var config = (ProvideConnectorTransportConfig)session.Config;
        var provide = config.Provide;
        var provideSession = (ProvideSession)session;
        provide.Sessions.Add(provideSession);
        
        session.Send(new BindProvide
        {
            info = new ProvideInfo
            {
                pvid = provide.Pvid,
                type = (int)provide.Type
            }
        });
    }

    public override void OnRemoveSession(Session session)
    {
        base.OnRemoveSession(session);
        
        var config = (ProvideConnectorTransportConfig)session.Config;
        var provide = config.Provide;
        var provideSession = (ProvideSession)session;
        provide.Sessions.Remove(provideSession);
    }
}