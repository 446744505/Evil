using System;
using Logic.Hero.Proto;
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
        public override async void OnAddSession(Session session)
        {
            base.OnAddSession(session);
            var heroService = new HeroService();
            var hero = await heroService.GetHero(1);
            Console.WriteLine(hero);
        }
    }
}