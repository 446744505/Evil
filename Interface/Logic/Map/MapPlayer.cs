using Attributes;

namespace Player
{
    [XTable(Node.Map, 500, typeof(MapPlayer),true)]
    public class MapPlayer
    {
        [XColumn(true)]
        private long playerId;
    }
}