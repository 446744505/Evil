
using Attributes;

namespace Hero
{
     [ClientToServer]
     public interface HeroService
     {
          /// <summary>
          /// 获取英雄信息
          /// </summary>
          /// <param name="heroId"></param>
          /// <returns></returns>
          public Hero HeroReq([ProtocolField(1)]long heroId);
          /// <summary>
          /// 英雄升星
          /// </summary>
          /// <param name="heroId"></param>
          /// <returns></returns>
          public HeroStarNtf HeroStarReq([ProtocolField(1)]long heroId);
     }
}
