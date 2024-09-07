using System.Collections.Generic;
using Attributes;

namespace Player
{
    [XTable(Node.Game, 500, typeof(User))]
    public class User
    {
        [XColumn(true)] 
        private string account;
        [XColumn] 
        private int userId;
        [XColumn] 
        private List<long> playerIds;
    }
}