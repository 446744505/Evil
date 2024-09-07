
using System.Threading.Tasks;
using Game.NetWork;

namespace Proto
{
    public partial class ListHeroes
    {
        public override async Task<PlayerHero> OnRequest()
        {
            var playerId = Net.I.PlayerId(this);
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
        public override async Task<Hero> OnRequest()
        {
            var playerId = Net.I.PlayerId(this);
            var ph = await XTable.PlayerHero.Select(playerId);
            if (ph != null)
            {
                if (ph.Heroes.TryGetValue(heroId, out var hero))
                {
                    return hero.ToProto();
                }
            }

            const int heroNotExist = 1;
            return CreateAck(heroNotExist);
        }
    }

    public partial class HeroStar
    {
        public override async Task<bool> Process()
        {
            var playerId = Net.I.PlayerId(this);
            var ph = await XTable.PlayerHero.Update(playerId);
            if (ph != null)
            {
                if (ph.Heroes.TryGetValue(heroId, out var hero))
                {
                    hero.Star += 1;
                    Net.I.SendToPlayerWhenCommit(playerId, new HeroStarNtf()
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