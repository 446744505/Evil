using Attributes;

namespace Hero
{
    [Protocol]
    public class Hero
    {
        [ProtocolField(1)]
        private long id;
        [ProtocolField(2)]
        private int level;
    }
}