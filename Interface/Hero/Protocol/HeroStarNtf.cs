using Attributes;

namespace Hero
{
    /// <summary>
    /// 升星返回
    /// </summary>
    [Protocol]
    public class HeroStarNtf
    {
        /// <summary>
        /// 英雄id
        /// </summary>
        [ProtocolField(1)]
        private long heroId;
        /// <summary>
        /// 升级后星级
        /// </summary>
        [ProtocolField(2)]
        private int star;
    }
}