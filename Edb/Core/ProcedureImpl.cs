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

        internal bool Call()
        {
            var savepoint = Transaction.Savepoint();
            try
            {
                if (m_Process is Procedure procedure)
                {
                    if (procedure.Process())
                    {
                        m_Exception = null;
                        m_Success = true;
                        return true;
                    }
                }
                else
                {
                    // TODO Function类型
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

        internal static IProcedure.IResult Call(TP p)
        {
            var impl = new ProcedureImpl<TP>(p);
            if (Transaction.Current == null)
            {
                try
                {
                    Transaction.Create();
                    for (var retry = 0;;)
                    {
                        if (Perform(impl))
                            break;
                        retry++;
                        if (retry > impl.RetryTimes)
                            break;
                        if (retry == impl.RetryTimes && impl.RetrySerial)
                            impl.IsolationLevel = IsolationLevel.Level3;

                        Task.Delay(impl.CalcDelay());
                    }
                }
                finally
                {
                    Transaction.Destroy();
                }
            }
            else
            {
                impl.Call();
            }

            return impl.Result;
        }

        internal static Task<IProcedure.IResult> Submit(TP p)
        {
            if (Transaction.Current != null)
                throw new ThreadStateException("can not submit procedure in transaction");
            var pt = new ProcedureTask(new ProcedureImpl<TP>(p));
            pt.Launch();
            return pt.PTask;
        }

        internal static void Execute(TP p, IProcedure.IDone<TP>? done)
        {
            new ProcedureTask(new ProcedureImpl<TP>(p), done).Launch();
        }

        internal int CalcDelay()
        {
            return Edb.I.Random.Next(RetryDelay);
        }

        private static bool Perform(ProcedureImpl<TP> p)
        {
            try
            {
                Transaction.Current!.Perform(p);
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