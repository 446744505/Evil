using System.Collections.Generic;
using Attributes;

namespace Hero
{
    [Protocol(Node.Client | Node.Game)]
    [XTable(Node.Game, 500, typeof(Player.Player))]
    public class PlayerHero
    {
        [XColumn(true)] 
        private long playerId;
        
        [ProtocolField(1)]
        [XColumn]
        private Dictionary<long, Hero> heroes;
    }
}