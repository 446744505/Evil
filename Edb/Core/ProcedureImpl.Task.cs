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
            }

            public async void Launch()
            {
                Interlocked.CompareExchange(ref m_CompletionSource, new TaskCompletionSource<IProcedure.IResult>(),
                    null);
                try
                {
                    // 用edb的任务接口执行，保证任务不会丢失
                    await Edb.I.Executor.ExecuteAsync(Start);
                }
                catch (Exception)
                {
                    // ignored 已经Done中设置在p.result中
                }
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

            private async void Start()
            {
                try
                {
                    try
                    {
                        await Transaction.Create().Perform(m_Procedure);
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