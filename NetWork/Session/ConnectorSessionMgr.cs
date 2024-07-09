namespace NetWork
{
    public class ConnectorSessionMgr : ISessionMgr
    {
        public Session? Session { get; set; }
        public virtual void OnAddSession(Session session)
        {
            lock (this)
            {
                if (Session != null)
                {
                    throw new NetWorkException($"had session {Session} already!");
                }
                Session = session;
            }
        }

        public virtual void OnRemoveSession(Session session)
        {
            lock (this)
            {
                Session?.OnClose();
                Session = null;
            }
        }
    }
}