using Attributes;

namespace Player
{
    [XTable(Node.Map, 500)]
    public class MapPlayer
    {
        [XColumn(true)]
        private long playerId;
    }
}