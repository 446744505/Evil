using System.Collections.Generic;
using Attributes;

namespace Logic.Hero
{
    [Protocol]
    public class Hero
    {
        [ProtocolField(1)]
        private long heroId;
        [ProtocolField(2)]
        private int star; // 星级
        [ProtocolField(3)]
        private List<HeroSkill> skills; // 技能
    }
}