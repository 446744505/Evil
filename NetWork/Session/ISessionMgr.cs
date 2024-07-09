namespace NetWork
{
    public interface ISessionMgr
    {
        public void OnAddSession(Session session);
        public void OnRemoveSession(Session session);
    }
}