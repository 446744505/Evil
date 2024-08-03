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
            var playerId = 99;
            var heroes = await m_HeroService.ListHeroes(playerId);
            Log.I.Debug($"list heroes: {heroes}");
            foreach (var pair in heroes.heroes)
            {
                var hero = await m_HeroService.GetHero(playerId, pair.Key);
                Log.I.Debug($"get hero: {hero}");
                m_HeroService.HeroStar(playerId, pair.Key);
            }
        }
    }
}