
using System.Threading.Tasks;
using Edb;

namespace Proto
{
    public partial class ListHeroes
    {
        public override async Task<PlayerHero> DeRequest()
        {
            Proto.PlayerHero? result = null;
            await Procedure.Submit(async () =>
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

                result = ph.ToProto();
                return true;
            });
            return result ?? new();
        }
    }
    public partial class GetHero
    {
        public override async Task<Hero> DeRequest()
        {
            Proto.Hero? result = null;
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Select(playerId);
                if (ph != null)
                {
                    if (ph.Heroes.TryGetValue(heroId, out var hero))
                    {
                        result = hero.ToProto();
                    }
                }
                
                return true;
            });

            return result ?? new();
        }
    }

    public partial class HeroStar
    {
        public override async void Process()
        {
            await Procedure.Submit(async () =>
            {
                var ph = await XTable.PlayerHero.Update(playerId);
                if (ph != null)
                {
                    if (ph.Heroes.TryGetValue(heroId, out var hero))
                    {
                        hero.Star += 1;
                        _ = Session.Send(new HeroStarNtf()
                        {
                            heroId = heroId,
                            star = hero.Star,
                        });
                    }
                }

                return true;
            });
        }
    }
}