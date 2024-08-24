using NetWork;
using Proto;

namespace Evil.Switcher
{
    public class LinkerSessionMgr : AcceptorSessionMgr
    {
        public override void OnAddSession(Session session)
        {
            var linker = Linker.I;
            var linkerSession = (LinkerSession)session;
            if (!linker.CanAddSession())
            {
                // 连接数超限
                _ = linker.CloseSession(linkerSession, SessionError.OverMaxSession);
                return;
            }
            base.OnAddSession(session);
            // TODO start key exchange
            linker.Sessions.AddSession(linkerSession);
        }

        public override void OnRemoveSession(Session session)
        {
            base.OnRemoveSession(session);
            var linkerSession = (LinkerSession)session;
            Linker.I.Sessions.RemoveSession(linkerSession);
            // TODO notify provider
        }
    }
}