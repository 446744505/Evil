using System.Collections.Concurrent;

namespace Evil.Util
{
    /// <summary>
    /// 1、可以优雅停止的任务执行器
    /// 2、处理所有异常不至于导致程序退出
    /// Task:会等所有加入的任务执行完成
    /// Delay Timer:会等正在执行的任务执行完毕，还没开始的不会等待执行
    /// Ticker:会等正在执行的任务执行完毕，还没开始的不会等待执行
    /// 有没有更好的实现方式？
    /// </summary>
    public class Executor
    {
        private readonly TimeProvider m_TimeProvider;

        private long m_NextId;
        private volatile bool m_IsDisposed;
        private readonly LockAsync m_Lock = new();
        
        private readonly ConcurrentDictionary<long, Task> m_Tasks = new();
        private readonly ConcurrentDictionary<long, ITimer> m_Tickers = new();

        private long m_RunningTimerCount;

        public Executor(TimeProvider? provider = null)
        {
            m_TimeProvider = provider ?? TimeProvider.System;
        }

        public Task ExecuteAsync(Action action)
        {
            CheckDisposed();
            var id = NewId();
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
                finally
                {
                    m_Tasks.Remove(id, out _);
                }
            });
            m_Tasks[id] = task;
            return task;
        }
        
        public Task ExecuteAsync(Func<Task> func)
        {
            CheckDisposed();
            var id = NewId();
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
                finally
                {
                    m_Tasks.Remove(id, out _);
                }
            });
            m_Tasks[id] = task;
            return task;
        }
        
        public Task<T?> ExecuteAsync<T>(Func<Task<T>> func)
        {
            CheckDisposed();
            var id = NewId();
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
                finally
                {
                    m_Tasks.Remove(id, out _);
                }

                return default;
            });
            m_Tasks[id] = task;
            return task;
        }
        
        public ITimer Delay(Action cb, int delay)
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
            return timer;
        }
        
        public ITimer Delay(Func<Task> cb, int delay)
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
            return timer;
        }

        public long Tick(Action cb, int dueTime, int period)
        {
            CheckDisposed();
            var id = NewId();
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
            m_Tickers[id] = timer;
            return id;
        }
        
        public long Tick(Func<Task> cb, int dueTime, int period)
        {
            CheckDisposed();
            var id = NewId();
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
            m_Tickers[id] = timer;
            return id;
        }

        public bool CancelTick(long id)
        {
            if (m_Tickers.TryRemove(id, out var timer))
            {
                timer.Dispose();
            }

            return false;
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
                if (m_IsDisposed)
                    return;
                
                m_IsDisposed = true;
                
                // 先停所有tick
                foreach (var tick in m_Tickers.Values)
                {
                    await tick.DisposeAsync();
                }
                // 等待所有正在执行的定时器任务执行完毕
                Log.I.Info($"running timer count: {Interlocked.Read(ref m_RunningTimerCount)}");
                while (Interlocked.Read(ref m_RunningTimerCount) > 0)
                {
                    await Task.Delay(100);
                }

                // 等待所有任务执行完毕
                Log.I.Info($"running task count: {m_Tasks.Count}");
                foreach (var task in m_Tasks.Values)
                {
                    await task;
                }
            } finally
            {
                m_Lock.WUnlock(release);
                Log.I.Info("executor stop end");
            }
        }

        private long NewId()
        {
            return Interlocked.Increment(ref m_NextId);
        }
    }
}