
using Attributes;

namespace Hero;
    
[ClientToServer]
public interface IHeroService
{
     void DoStar(long id);
}
