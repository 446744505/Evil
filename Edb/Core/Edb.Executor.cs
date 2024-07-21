using System.Collections.Concurrent;

namespace Edb
{
    public partial class Edb
    {
        private volatile bool m_IsDisposed;
        private ConcurrentBag<Task> m_Tasks = new();
        // 用于控制优雅停服
        private long m_RunningTimerCount;
        private ConcurrentBag<Timer> m_Timers = new();
        public Task ExecuteAsync(Action action)
        {
            CheckDisposed();
            var task = Task.Run(action);
            m_Tasks.Add(task);
            return task;
        }

        public void Tick(Action cb, int dueTime, int period)
        {
            CheckDisposed();
            var timer = new Timer(_ =>
            {
                Interlocked.Increment(ref m_RunningTimerCount);
                try
                {
                    cb();
                }
                finally
                {
                    Interlocked.Decrement(ref m_RunningTimerCount);
                }
            }, null, dueTime, period);
            m_Timers.Add(timer);
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
        }
    }
}