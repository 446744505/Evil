
using Attributes;

namespace Player
{
    [XTable(Node.Game, 500, typeof(Player))]
    public class Player
    {
        [XColumn(true)]
        private long playerId;

        [XColumn]
        private int serverId;
        
        [XColumn]
        private string playerName;
        
        [XColumn]
        [XListener]
        private int level;
    }
}