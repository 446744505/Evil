using Evil.Util;

namespace Edb
{
    public sealed partial class Edb : Singleton<Edb>
    {
        public Config Config { get; set; }
        public Random Random => m_Random.Value!;
        
        private readonly ThreadLocal<Random> m_Random = new(() => new Random());
    }
}