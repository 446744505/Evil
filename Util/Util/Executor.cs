using System.Collections.Concurrent;

namespace Evil.Util
{
    /// <summary>
    /// 1、可以优雅停止的任务执行器
    /// 2、处理所有异常不至于导致程序退出
    /// </summary>
    public class Executor
    {
        private readonly TimeProvider m_TimeProvider;
        
        private volatile bool m_IsDisposed;
        private ConcurrentBag<Task> m_Tasks = new();
        private LockAsync m_Lock = new();
        private long m_RunningTimerCount;
        private ConcurrentBag<ITimer> m_Timers = new();

        public Executor(TimeProvider? provider = null)
        {
            if (provider == null)
                provider = TimeProvider.System;
            else
                m_TimeProvider = provider;
        }
        
        public Task ExecuteAsync(Action action)
        {
            CheckDisposed();
            var task = Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Log.I.Error(e);
                }
            });
            m_Tasks.Add(task);
            return task;
        }
        
        public Task ExecuteAsync(Func<Task> func)
        {
            CheckDisposed();
            var task = Task.Run(async () =>
            {
                try
                {
                    await func();
                }
                catch (Exception e)
                {
                    Log.I.Error(e);
                }
            });
            m_Tasks.Add(task);
            return task;
        }
        
        public Task<T?> ExecuteAsync<T>(Func<Task<T>> func)
        {
            CheckDisposed();
            var task = Task.Run<T?>(async () =>
            {
                try
                {
                    return await func();
                }
                catch (Exception e)
                {
                    Log.I.Error(e);
                }

                return default;
            });
            m_Tasks.Add(task);
            return task;
        }
        
        public void Delay(Action cb, int delay)
        {
            CheckDisposed();
            var timer = m_TimeProvider.CreateTimer(_ =>
            {
                Interlocked.Increment(ref m_RunningTimerCount);
                try
                {
                    cb();
                } 
                catch (Exception e)
                {
                    Log.I.Error(e);
                }
                finally
                {
                    Interlocked.Decrement(ref m_RunningTimerCount);
                }
            }, null, TimeSpan.FromMilliseconds(delay), Timeout.InfiniteTimeSpan);
            
            m_Timers.Add(timer);
        }
        
        public void Delay(Func<Task> cb, int delay)
        {
            CheckDisposed();
            var timer = m_TimeProvider.CreateTimer(async _ =>
            {
                Interlocked.Increment(ref m_RunningTimerCount);
                try
                {
                    await cb();
                } 
                catch (Exception e)
                {
                    Log.I.Error(e);
                }
                finally
                {
                    Interlocked.Decrement(ref m_RunningTimerCount);
                }
            }, null, TimeSpan.FromMilliseconds(delay), Timeout.InfiniteTimeSpan);
            m_Timers.Add(timer);
        }

        public void Tick(Action cb, int dueTime, int period)
        {
            CheckDisposed();
            var timer = m_TimeProvider.CreateTimer(_ =>
            {
                Interlocked.Increment(ref m_RunningTimerCount);
                try
                {
                    cb();
                }
                catch (Exception e)
                {
                    Log.I.Error(e);
                }
                finally
                {
                    Interlocked.Decrement(ref m_RunningTimerCount);
                }
            }, null, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
            m_Timers.Add(timer);
        }
        
        public void Tick(Func<Task> cb, int dueTime, int period)
        {
            CheckDisposed();
            var timer = m_TimeProvider.CreateTimer( async _ =>
            {
                Interlocked.Increment(ref m_RunningTimerCount);
                try
                {
                    await cb();
                }
                catch (Exception e)
                {
                    Log.I.Error(e);
                }
                finally
                {
                    Interlocked.Decrement(ref m_RunningTimerCount);
                }
            }, null, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
            m_Timers.Add(timer);
        }
        
        private void CheckDisposed()
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(nameof(Executor));
            }
        }
        
        /// <summary>
        /// 关闭时要等待所有任务执行完毕
        /// </summary>
        public async Task DisposeAsync()
        {
            Log.I.Info("executor start stop");
            IDisposable? release = await m_Lock.WLockAsync();
            try
            {
                if (!m_IsDisposed)
                    return;
                
                m_IsDisposed = true;
                // 等待所有的定时器任务执行完毕
                Log.I.Info($"running timer count: {Interlocked.Read(ref m_RunningTimerCount)}");
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
                Log.I.Info($"running task count: {m_Tasks.Count}");
                foreach (var task in m_Tasks)
                {
                    await task;
                }
            } finally
            {
                m_Lock.WUnlock(release);
                Log.I.Info("executor stop end");
            }
        }
    }
}