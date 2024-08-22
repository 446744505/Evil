using NetWork;

namespace Evil.Switcher
{
    public class ProviderSessionMgr : AcceptorSessionMgr
    {
        public override void OnRemoveSession(Session session)
        {
            base.OnRemoveSession(session);
            Provider.I.Sessions.UnBind((ProviderSession)session);
        }
    }
}