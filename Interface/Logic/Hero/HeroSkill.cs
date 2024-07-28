using Attributes;

namespace Hero
{
    [Protocol(Node.Client | Node.Game)]
    [XBean(Node.Game)]
    public class HeroSkill
    {
        [ProtocolField(1)]
        [XColumn]
        private int cfgId;
        
        [ProtocolField(2)]
        [XColumn]
        private int level;
    }
}