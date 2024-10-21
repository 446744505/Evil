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

            public async Task Launch()
            {
                Interlocked.CompareExchange(ref m_CompletionSource, new TaskCompletionSource<IProcedure.IResult>(),
                    null);
               
                // 用edb的任务接口执行，保证任务不会丢失
                await Edb.I.Executor.ExecuteAsync(Start);
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

            private async Task Start()
            {
                var ctx = TransactionCtx.Create().Start();
                try
                {
                    try
                    {
                        await ctx.Current!.Perform(m_Procedure, ctx);
                    }
                    finally
                    {
                        ctx.Destroy();
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