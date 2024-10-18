using System.Collections.Concurrent;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;

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
        private long m_NextId;
        private readonly ConcurrentDictionary<long, IScheduledTask> m_Tickers = new();
        
        private readonly TimeProvider m_TimeProvider;
        
        private readonly MultithreadEventLoopGroup m_EventLoopGroup = new();

        public Executor(TimeProvider? provider = null)
        {
            m_TimeProvider = provider ?? TimeProvider.System;
        }

        public void Execute(Action action)
        {
            m_EventLoopGroup.Execute(action);
        }
        
        public Task<T> SubmitAsync<T>(Func<T> func)
        {
            return m_EventLoopGroup.SubmitAsync(func);
        }
        
        // public Task<T?> ExecuteAsync<T>(Func<Task<T>> func)
        // {
        //     var tcs = new TaskCompletionSource<T?>();
        //     m_EventLoopGroup.Execute(async () =>
        //     {
        //         try
        //         {
        //             tcs.SetResult(await func());
        //         }
        //         catch (Exception e)
        //         {
        //             tcs.SetException(e);
        //         }
        //     });
        //     return tcs.Task;
        // }
        
        public IScheduledTask Delay(Action cb, int delay)
        {
            return m_EventLoopGroup.Schedule(cb, TimeSpan.FromMilliseconds(delay));
        }
        
        // public IScheduledTask Delay(Func<Task> cb, int delay)
        // {
        //     return m_EventLoopGroup.Schedule(async () =>
        //     {
        //         try
        //         {
        //             await cb();
        //         }
        //         catch (Exception e)
        //         {
        //             
        //         }
        //     }, TimeSpan.FromMilliseconds(delay));
        // }

        private long NextTick(Action cb, int delay, int period, long id)
        {
            var isNew = id == 0;
            if (isNew)
                id = NewId();
            else
                delay = period;
            
            var task = m_EventLoopGroup.Schedule(() =>
            {
                try
                {
                    cb();
                }
                finally
                {
                    NextTick(cb, delay, period, id);
                }
            }, TimeSpan.FromMilliseconds(delay));
            m_Tickers[id] = task;
            return id;
        }

        public long Tick(Action cb, int dueTime, int period)
        {
            return NextTick(cb, dueTime, period, 0);
        }

        public bool CancelTick(long id)
        {
            if (m_Tickers.TryRemove(id, out var task))
            {
                return task.Cancel();
            }

            return false;
        }
        
        /// <summary>
        /// 关闭时要等待所有任务执行完毕
        /// </summary>
        public void Dispose()
        {
            Log.I.Info("executor start stop");
            m_EventLoopGroup.ShutdownGracefullyAsync().Wait();
            m_Tickers.Clear();
            Log.I.Info("executor end stop");
        }

        private long NewId()
        {
            return Interlocked.Increment(ref m_NextId);
        }
    }
}