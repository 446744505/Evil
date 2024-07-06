using Attributes;

namespace Logic.Hero
{
    [Protocol]
    public class Hero
    {
        [ProtocolField(1)]
        private long heroId;
        [ProtocolField(2)]
        private int star;
        [ProtocolField(3)]
        private List<HeroSkill> skills;
    }
}