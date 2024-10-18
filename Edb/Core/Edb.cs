using Evil.Util;

namespace Edb
{
    public sealed partial class Edb : Singleton<Edb>
    {
        private volatile Tables? m_Tables;
        private bool m_Running;
        private volatile Checkpoint? m_Checkpoint;
        private Executor m_Executor = new();

        public Config Config { get; set; } = null!;
        internal Tables Tables => m_Tables!;
        public Random Random => m_Random.Value!;
        public Executor Executor => m_Executor;
        
        private readonly ThreadLocal<Random> m_Random = new(() => new Random());

        public void Start(Config config, List<BaseTable> tables)
        {
            Log.I.Info("edb start begin");
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                Dispose();
            };
            lock (this)
            {
                try
                {
                    Config = config;
                    m_Tables = new();
                    foreach (var table in tables)
                        m_Tables.Add(table);
                    m_Tables.Open(config);
                    m_Checkpoint = new(m_Tables);
                    m_Running = true;
                    Log.I.Info("edb start end");
                }
                catch (Exception e)
                {
                    Log.I.Fatal(e);
                    Dispose();
                    throw;
                }
            }
        }

        public void CheckpointNow()
        {
            m_Checkpoint!.CheckpointNow();
        }

        public void Dispose()
        {
            Log.I.Info("edb start stop");
            lock (this)
            {
                if (!m_Running)
                    return;

                // 等待所有任务执行完毕
                m_Executor.Dispose();

                // 执行最后一次checkpoint
                if (m_Checkpoint != null)
                {
                    m_Checkpoint.Cleanup();
                    m_Checkpoint = null;
                }

                if (m_Tables != null)
                {
                    m_Tables.Dispose();
                    m_Tables = null!;
                }
                m_Running = false;
                Log.I.Info("edb end stop");   
            }
        }
    }
}