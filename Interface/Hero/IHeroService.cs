
using Protocol.Attributes;

namespace Protocol.Hero;
    
[Rpc]
public interface IHeroService
{
     Task<Hero> GetHero(long id);
}
