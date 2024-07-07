using System.Collections.Generic;
using Attributes;

namespace Logic.Player
{
    [Protocol]
    public class Player
    {
        [ProtocolField(1)]
        private long playerId;
        [ProtocolField(2)]
        private string playerName;
        [ProtocolField(3)]
        private int level;
        [ProtocolField(4)]
        private Dictionary<long, Hero.Hero> heroes;
    }
}