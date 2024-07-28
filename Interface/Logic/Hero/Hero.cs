using System.Collections.Generic;
using Attributes;

namespace Hero
{
    [Protocol(Node.Client|Node.Game)]
    [XBean(Node.Game)]
    public class Hero
    {
        [ProtocolField(1)]
        [XColumn]
        private long heroId;
        
        [ProtocolField(2)]
        [XColumn]
        private int star; // 星级
        
        [ProtocolField(3)]
        [XColumn]
        private List<HeroSkill> skills; // 技能
    }
}