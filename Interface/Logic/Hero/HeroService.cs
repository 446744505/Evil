
using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;

namespace Logic.Hero
{
     [ClientToServer]
     public interface HeroService
     {
          /// <summary>
          /// 获取英雄信息
          /// </summary>
          /// <param name="heroId"></param>
          /// <returns></returns>
          public Hero GetHero([ProtocolField(1)]long heroId);
          /// <summary>
          /// 获取所有英雄信息
          /// </summary>
          /// <returns></returns>
          public List<Hero> ListHeroes();
          /// <summary>
          /// 英雄升星
          /// </summary>
          /// <param name="heroId"></param>
          /// <returns></returns>
          public HeroStarAck HeroStar([ProtocolField(1)]long heroId);
     }
}
