
using System.Collections.Generic;
using Attributes;

namespace Hero
{
     [ClientToServer]
     public interface HeroService
     {
          public Hero AddHero([ProtocolField(1)]long heroId);
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
          public Heroes ListHeroes();
          /// <summary>
          /// 英雄升星
          /// </summary>
          /// <param name="heroId"></param>
          /// <returns></returns>
          public void HeroStar([ProtocolField(1)]long heroId);
     }
}
