using System.Threading.Tasks;
using Evil.Util;
using Proto;

namespace Client.Hero
{
    public class HeroMgr : Singleton<HeroMgr>
    {
        public readonly HeroService HeroService = new();

        public async Task Test()
        {
            var heroes = await HeroService.ListHeroes();
            Log.I.Debug($"list heroes: {heroes}");
            foreach (var pair in heroes.heroes)
            {
                var hero = await HeroService.GetHero(pair.Key);
                Log.I.Debug($"get hero: {hero}");
                HeroService.HeroStar(pair.Key);
            }
        }
    }
}