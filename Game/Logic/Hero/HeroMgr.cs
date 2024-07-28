using System.Collections.Generic;
using Evil.Util;

namespace Hero
{
    public class HeroMgr : Singleton<HeroMgr>
    {
        public readonly Proto.Hero HeroNull = new();
        private readonly Dictionary<long, Proto.Hero> m_Heroes = new();
        
        public void AddHero(Proto.Hero hero)
        {
            m_Heroes[hero.heroId] = hero;
        }
        
        public Proto.Hero? GetHero(long heroId)
        {
            m_Heroes.TryGetValue(heroId, out var hero);
            return hero;
        }
    }
}