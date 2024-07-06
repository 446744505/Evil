using Attributes;

namespace Logic.Hero
{
    [Protocol]
    public class HeroSkill
    {
        [ProtocolField(1)]
        private int cfgId;
        [ProtocolField(2)]
        private int level;
    }
}