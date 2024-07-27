using Evil.Util;

namespace Edb
{
    public sealed partial class Edb : Singleton<Edb>
    {
        private volatile Tables m_Tables = null!;
        private bool m_Running;
        private volatile Checkpoint? m_Checkpoint;
        private LockAsync m_Lock = new();

        public Config Config { get; set; } = null!;
        internal Tables Tables => m_Tables;
        public Random Random => m_Random.Value!;
        
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
                await DisposeAsync();
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

        private void CheckDisposed()
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(nameof(Edb));
            }
        }

        public async Task DisposeAsync()
        {
            var release = await m_Lock.WLockAsync();
            try
            {
                if (!m_Running)
                    return;
                
                m_IsDisposed = true;
                // 等待所有的定时器任务执行完毕
                while (Interlocked.Read(ref m_RunningTimerCount) > 0)
                {
                    await Task.Delay(100);
                }

                // 放在后面，否者直接关闭timer会导致在执行的任务无法执行完毕
                foreach (var timer in m_Timers)
                {
                    await timer.DisposeAsync();
                }

                // 等待所有任务执行完毕
                foreach (var task in m_Tasks)
                {
                    await task;
                }

                // 执行最后一次checkpoint
                if (m_Checkpoint != null)
                {
                    await m_Checkpoint.Cleanup();
                    m_Checkpoint = null;
                }
                m_Running = false;
            } finally
            {
                m_Lock.WUnlock(release);
            }
        }
    }
}