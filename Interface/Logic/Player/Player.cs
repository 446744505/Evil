using System.Collections.Generic;
using Attributes;

namespace Player
{
    [XTable(Node.Game, 500)]
    public class Player
    {
        [XColumn(true)]
        private long playerId;
        
        [XColumn]
        private string playerName;
        
        [XColumn]
        private int level;
        
        [XColumn]
        private Dictionary<long, Hero.Hero> heroes;
    }
}