using Attributes;

namespace Logic.Hero
{
    [Protocol(Node.Client | Node.Game)]
    public class HeroSkill
    {
        [ProtocolField(1)]
        private int cfgId;
        [ProtocolField(2)]
        private int level;
    }
}