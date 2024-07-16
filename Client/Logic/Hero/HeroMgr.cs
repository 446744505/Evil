using Evil.Util;
using Logic.Hero.Proto;

namespace Client.Logic.Hero;

public class HeroMgr : Singleton<HeroMgr>
{
    private readonly HeroService m_HeroService = new();

    public async void Test()
    {
        var newHero = await m_HeroService.AddHero(1);
        Log.I.Debug($"new hero: {newHero.heroId}");
        var hero = await m_HeroService.GetHero(newHero.heroId);
        Log.I.Debug($"get hero: {hero.heroId}");
        m_HeroService.HeroStar(newHero.heroId);
    }
}