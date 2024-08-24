using NetWork;
using Proto;

namespace Evil.Provide;

public class ProvideSessionMgr : ConnectorSessionMgr
{
    public override void OnAddSession(Session session)
    {
        base.OnAddSession(session);
        session.SendAsync(new BindProvide {pvid = session.Config.Pvid});
    }
}