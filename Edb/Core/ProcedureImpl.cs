using Evil.Util;

namespace Edb
{
    internal sealed partial class ProcedureImpl<TP> where TP : IProcedure
    {
        private volatile bool m_Success;
        private volatile Exception? m_Exception;
        private readonly TP m_Process;
        
        internal string Name => m_Process.Name;
        internal TP Process => m_Process;
        internal IsolationLevel IsolationLevel { get; set; }
        internal int RetryTimes => m_Process.RetryTimes == -1 ? Edb.I.Config.RetryTimes : m_Process.RetryTimes;
        private int RetryDelay => m_Process.RetryDelay == -1 ? Edb.I.Config.RetryDelay : m_Process.RetryDelay;
        internal bool RetrySerial => m_Process.RetrySerial || Edb.I.Config.RetrySerial;
        internal IProcedure.IResult Result => new IProcedure.ResultImpl(m_Success, m_Exception);
        
        internal bool Success
        {
            get => m_Success;
            set => m_Success = value;
        }

        internal Exception? Exception
        {
            get => m_Exception;
            set => m_Exception = value;
        }
        
        private ProcedureImpl(TP p)
        {
            m_Process = p;
            IsolationLevel = Transaction.IsolationLevel;
        }

        internal async Task<bool> Call()
        {
            var savepoint = Transaction.Savepoint();
            try
            {
                if (m_Process is Procedure procedure)
                {
                    if (await procedure.Process())
                    {
                        m_Exception = null;
                        m_Success = true;
                        return true;
                    }
                }
                else
                {
                    throw new NotSupportedException(Name);
                }
            }
            catch (LockTimeoutException)
            {
                // 拦截掉LockTimeoutException重试
                throw;
            }
            catch (Exception e)
            {
                m_Exception = e;
                Log.I.Error(e);
            }

            Transaction.Rollback(savepoint);
            m_Success = false;
            return false;
        }

        internal static async Task<IProcedure.IResult> Call(TP p)
        {
            var impl = new ProcedureImpl<TP>(p);
            if (Transaction.Current == null)
            {
                try
                {
                    Transaction.Create();
                    for (var retry = 0;;)
                    {
                        if (await Perform(impl))
                            break;
                        retry++;
                        if (retry > impl.RetryTimes)
                            break;
                        if (retry == impl.RetryTimes && impl.RetrySerial)
                            impl.IsolationLevel = IsolationLevel.Level3;

                        await Task.Delay(impl.CalcDelay());
                    }
                }
                finally
                {
                    Transaction.Destroy();
                }
            }
            else
            {
                await impl.Call();
            }

            return impl.Result;
        }

        internal static Task<IProcedure.IResult> Submit(TP p)
        {
            if (Transaction.Current != null)
                throw new ThreadStateException("can not submit procedure in transaction");
            var pt = new ProcedureTask(new ProcedureImpl<TP>(p));
            pt.Launch().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    Log.I.Error(task.Exception);
            
            });
            return pt.PTask;
        }

        internal static void Execute(TP p, Action<TP,IProcedure.IResult>? done)
        {
            new ProcedureTask(new ProcedureImpl<TP>(p), done).Launch().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    Log.I.Error(task.Exception);
            
            });
        }

        internal int CalcDelay()
        {
            return Edb.I.Random.Next(RetryDelay>>1, RetryDelay<<1);
        }

        private static async Task<bool> Perform(ProcedureImpl<TP> p)
        {
            try
            {
                await Transaction.Current!.Perform(p);
            }
            catch (LockTimeoutException)
            {
                return false;
            }
            catch (Exception)
            {
                // ignored
            }

            return true;
        }

        
    }
}