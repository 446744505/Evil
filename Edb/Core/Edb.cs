using Evil.Util;

namespace Edb
{
    public sealed partial class Edb : Singleton<Edb>, IDisposable
    {
        private volatile Tables m_Tables;
        private volatile bool m_Running;
        
        public Config Config { get; set; }
        internal Tables Tables => m_Tables;
        public Random Random => m_Random.Value!;
        
        private readonly ThreadLocal<Random> m_Random = new(() => new Random());

        public void Start(Config config, List<BaseTable> tables)
        {
            Log.I.Info("edb start begin");
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                Dispose();
            };
            Config = config;
            m_Tables = new();
            try
            {
                foreach (var table in tables)
                    m_Tables.Add(table);
                m_Tables.Open(config);
                m_Running = true;
                Log.I.Info("edb start end");
            } catch (Exception e)
            {
                Log.I.Fatal(e);
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                
            }
        }
    }
}