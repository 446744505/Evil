using System;

namespace Attributes
{
    [Flags]
    public enum Node
    {
        Client = 1,
        Switcher = 2,
        Game = 4,
        Map = 8,
    }
}