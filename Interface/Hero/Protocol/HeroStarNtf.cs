using Attributes;

namespace Hero
{
    /// <summary>
    /// 升星返回
    /// </summary>
    [Protocol]
    public class HeroStarNtf
    {
        [ProtocolField(1)]
        private long heroId; // 英雄id
        [ProtocolField(2)]
        private int star; // 升级后星级
    }
}