using Evil.Util;

namespace Edb
{
    public sealed partial class Edb : Singleton<Edb>
    {
        private volatile Tables m_Tables = null!;
        private bool m_Running;
        private volatile Checkpoint? m_Checkpoint;
        private Executor m_Executor = new();
        private LockAsync m_Lock = new();

        public Config Config { get; set; } = null!;
        internal Tables Tables => m_Tables;
        public Random Random => m_Random.Value!;
        public Executor Executor => m_Executor;
        
        private readonly ThreadLocal<Random> m_Random = new(() => new Random());

        public async Task Start(Config config, List<BaseTable> tables)
        {
            Log.I.Info("edb start begin");
            AppDomain.CurrentDomain.ProcessExit += async (sender, args) =>
            {
                await DisposeAsync();
            };
            var release = await m_Lock.WLockAsync();
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
                await DisposeAsync(true);
                throw;
            }
            finally
            {
                m_Lock.WUnlock(release);
            }
        }

        public async Task CheckpointNow()
        {
            await m_Checkpoint!.CheckpointNow();
        }

        public async Task DisposeAsync(bool locked = false)
        {
            Log.I.Info("edb start stop");
            IDisposable? release = null;
            if (!locked)
                await m_Lock.WLockAsync();
            try
            {
                if (!m_Running)
                    return;

                // 等待所有任务执行完毕
                await m_Executor.DisposeAsync();

                // 执行最后一次checkpoint
                if (m_Checkpoint != null)
                {
                    await m_Checkpoint.Cleanup();
                    m_Checkpoint = null;
                }

                if (m_Tables != null)
                {
                    m_Tables.Dispose();
                    m_Tables = null!;
                }
                m_Running = false;
            } finally
            {
                if (release != null)
                    m_Lock.WUnlock(release);
                Log.I.Info("edb stop end");
            }
        }
    }
}