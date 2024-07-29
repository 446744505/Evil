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
        [XColumn]
        [ProtocolField(1)]
        private List<Hero> heroes;
    }
}