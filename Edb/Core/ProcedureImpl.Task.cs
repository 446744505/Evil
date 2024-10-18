using Evil.Util;

namespace Edb
{
    internal sealed partial class ProcedureImpl<TP> where TP : IProcedure
    {
        internal class ProcedureTask
        {
            private volatile TaskCompletionSource<IProcedure.IResult>? m_CompletionSource;
            private readonly ProcedureImpl<TP> m_Procedure;
            private readonly Action<TP,IProcedure.IResult>? m_Done;
            private volatile int m_Retry;
            
            internal Task<IProcedure.IResult> PTask => m_CompletionSource!.Task;

            public ProcedureTask(ProcedureImpl<TP> procedure, Action<TP,IProcedure.IResult>? done = null)
            {
                m_Procedure = procedure;
                m_Done = done;
                Launch();
            }

            private void Launch()
            {
                Interlocked.CompareExchange(ref m_CompletionSource, new TaskCompletionSource<IProcedure.IResult>(),
                    null);
                
                Edb.I.Executor.Execute(Start);
            }

            private void Done()
            {
                m_CompletionSource!.SetResult(m_Procedure.Result);
                
                if (m_Done == null)
                    return;
                try
                {
                    m_Done(m_Procedure.Process, m_Procedure.Result);     
                } catch (Exception e)
                {
                    Log.I.Error(e);
                }
            }

            private void Start()
            {
                try
                {
                    try
                    {
                        Transaction.Create().Perform(m_Procedure);
                    }
                    finally
                    {
                        Transaction.Destroy();
                    }
                    
                    Done();
                }
                catch (LockTimeoutException)
                {
                    m_Retry = Interlocked.Increment(ref m_Retry);
                    if (m_Retry > m_Procedure.RetryTimes)
                    {
                        Done();
                        throw;
                    }

                    if (m_Retry == m_Procedure.RetryTimes && m_Procedure.RetrySerial)
                        m_Procedure.IsolationLevel = IsolationLevel.Level3;
                    Edb.I.Executor.Delay(Launch, m_Procedure.CalcDelay());
                }
                catch (XError)
                {
                    Done();
                    throw;
                }
                catch (Exception e)
                {
                    Done();
                    throw new XError("", e);
                }
            }
        }
    }
}