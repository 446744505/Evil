using Evil.Util;

namespace Edb
{
    public sealed class Edb : Singleton<Edb>
    {
        public Config Config { get; set; }
    }
}