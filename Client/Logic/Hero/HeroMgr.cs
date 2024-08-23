using System.Threading.Tasks;
using Evil.Util;
using Proto;

namespace Client.Hero
{
    public class HeroMgr : Singleton<HeroMgr>
    {
        private readonly HeroService m_HeroService = new();

        public async Task Test()
        {
            var heroes = await m_HeroService.ListHeroes();
            Log.I.Debug($"list heroes: {heroes}");
            foreach (var pair in heroes.heroes)
            {
                var hero = await m_HeroService.GetHero(pair.Key);
                Log.I.Debug($"get hero: {hero}");
                m_HeroService.HeroStar(pair.Key);
            }
        }
    }
}