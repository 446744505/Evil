
using System.Threading.Tasks;
using Edb;
using Evil.Util;

namespace Proto
{
    public partial class ListHeroes
    {
        public override async Task<PlayerHero> DeRequest()
        {
            var ph = await XTable.PlayerHero.Update(playerId);
            if (ph == null)
            {
                var hero = new XBean.Hero()
                {
                    HeroId = 1,
                    Star = 1,
                    Properties =
                    {
                        Abs = 1,
                        Pct = 1,
                    },
                    Skills = { new XBean.HeroSkill()
                    {
                        CfgId = 1,
                        Level = 1,
                    } }
                };

                ph = new XBean.PlayerHero()
                {
                    PlayerId = playerId,
                    Heroes =
                    {
                        { hero.HeroId, hero }
                    }
                };
                await XTable.PlayerHero.Insert(ph);
            }

            return ph.ToProto();
        }
    }
    public partial class GetHero
    {
        public override async Task<Hero> DeRequest()
        {
            var ph = await XTable.PlayerHero.Select(playerId);
            if (ph != null)
            {
                if (ph.Heroes.TryGetValue(heroId, out var hero))
                {
                    return hero.ToProto();
                }
            }

            return new Hero();
        }
    }

    public partial class HeroStar
    {
        public override async Task<bool> Process()
        {
            var ph = await XTable.PlayerHero.Update(playerId);
            if (ph != null)
            {
                if (ph.Heroes.TryGetValue(heroId, out var hero))
                {
                    hero.Star += 1;
                    ProcedureHelper.SendWhenCommit(Session, new HeroStarNtf()
                    {
                        heroId = heroId,
                        star = hero.Star,
                    });
                }
            }

            return true;
        }
    }
}