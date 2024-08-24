
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
        [XListener]
        private int level;
    }
}