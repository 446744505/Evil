using System.Collections.Generic;
using Attributes;

namespace Server
{
    [XTable(Node.Game, 500, typeof(Server))]
    public class Server
    {
        [XColumn(true)] 
        private int serverId;

        [XColumn] 
        private List<long> playerIds;
    }
}