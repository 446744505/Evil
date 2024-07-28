using System;

namespace Attributes
{
    [Flags]
    public enum Node
    {
        Client = 1,
        Game = 2,
        Map = 4,
    }
}