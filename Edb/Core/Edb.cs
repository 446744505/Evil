using Evil.Util;

namespace Edb
{
    public sealed partial class Edb : Singleton<Edb>
    {
        private volatile Tables m_Tables;
        
        public Config Config { get; set; }
        internal Tables Tables => m_Tables;
        public Random Random => m_Random.Value!;
        
        private readonly ThreadLocal<Random> m_Random = new(() => new Random());

        public void Start()
        {
            Config ??= new Config();
            m_Tables = new();
        }
    }
}