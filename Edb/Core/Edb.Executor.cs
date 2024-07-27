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
        
        public void Delay(Action cb, int delay)
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
            }, null, delay, -1);
            m_Timers.Add(timer);
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
    }
}