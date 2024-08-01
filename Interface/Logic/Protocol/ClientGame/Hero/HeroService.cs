using Attributes;

namespace Hero
{
     [Service(Node.Client, Node.Game)]
     public interface HeroService
     {
          /// <summary>
          /// 获取英雄信息
          /// </summary>
          /// <param name="heroId"></param>
          /// <returns></returns>
          public Hero GetHero([ProtocolField(1)]long playerId, [ProtocolField(2)]long heroId);
          /// <summary>
          /// 获取所有英雄信息
          /// </summary>
          /// <returns></returns>
          public PlayerHero ListHeroes([ProtocolField(1)]long playerId);
          /// <summary>
          /// 英雄升星
          /// </summary>
          /// <param name="heroId"></param>
          /// <returns></returns>
          public void HeroStar([ProtocolField(1)]long playerId, [ProtocolField(2)]long heroId);
     }
}
