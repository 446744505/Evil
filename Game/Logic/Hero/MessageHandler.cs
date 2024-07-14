namespace Logic.Hero.Proto
{
    public partial class AddHero
    {
        public override Hero DeRequest()
        {
            var hero = new Hero()
            {
                heroId = heroId,
                Star = 1,
            };
            HeroMgr.I.AddHero(hero);
            return hero;
        }
    }
    public partial class GetHero
    {
        public override Hero DeRequest()
        {
            var hero = HeroMgr.I.GetHero(heroId);
            if (hero != null)
            {
                return hero;
            }

            return HeroMgr.I.HeroNull;
        }
    }

    public partial class HeroStar
    {
        public override void Process()
        {
            var hero = HeroMgr.I.GetHero(heroId);
            if (hero != null)
            {
                hero.Star += 1;
                Session.Send(new HeroStarNtf()
                {
                    heroId = heroId,
                    Star = hero.Star,
                });
            }
        }
    }
}