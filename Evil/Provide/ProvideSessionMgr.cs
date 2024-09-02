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
        session.SendAsync(new BindProvide
        {
            pvid = provide.Pvid,
            type = (int)provide.Type
        });
    }
}