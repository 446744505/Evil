namespace Edb
{
    /// <summary>
    /// 执行完后可携带结果的Procedure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RProcedure<T> : Procedure
    {
        public T R { get; protected set; }
        public abstract Task<bool> Process(TransactionCtx ctx);
    }
    public interface Procedure : IProcedure
    {
        public static readonly Task<bool> FalseTask = Task.FromResult(false);
        public static readonly Task<bool> TrueTask = Task.FromResult(true);
        
        public Task<bool> Process(TransactionCtx ctx);
        
        public void Execute()
        {
            Execute(this);
        }
        
        public Task<IResult> Call(TransactionCtx ctx)
        {
            return Call(this, ctx);
        }
        
        public Task Submit()
        {
            return Submit(this);
        }

        static void Execute<TP>(TP p, Action<TP,IResult>? done = null) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, done);
        }
        
        static void Execute(Func<bool> p, string name = "")
        {
            Execute(new ProcedureInner(p, name));
        }
        
        static void Exceute(Func<TransactionCtx, Task<bool>> p, string name = "")
        {
            Execute(new ProcedureInnerAsync(p, name));
        }
        
        static Task<IResult> Call<TP>(TP p, TransactionCtx ctx) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Call(p, ctx);
        }
        
        static Task<IResult> Call(Func<bool> p, TransactionCtx ctx, string name = "")
        {
            return Call(new ProcedureInner(p, name), ctx);
        }
        
        static Task<IResult> Call(Func<TransactionCtx, Task<bool>> p, TransactionCtx ctx, string name = "")
        {
            return Call(new ProcedureInnerAsync(p, name), ctx);
        }
        
        static Task<IResult> Submit<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Submit(p);
        }
        
        static Task<IResult> Submit(Func<bool> p, string name = "")
        {
            return Submit(new ProcedureInner(p, name));
        }
        
        static Task<IResult> Submit(Func<TransactionCtx, Task<bool>> p, string name = "")
        {
            return Submit(new ProcedureInnerAsync(p, name));
        }

        private struct ProcedureInner : Procedure
        {
            private readonly string m_Name;
            private readonly Func<bool> m_Func;
            public string Name => m_Name;

            public ProcedureInner(Func<bool> func, string name)
            {
                m_Func = func;
                m_Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            }

            public Task<bool> Process(TransactionCtx ctx)
            {
                var success = m_Func();
                return success ? TrueTask : FalseTask;
            }
        }
        
        private struct ProcedureInnerAsync : Procedure
        {
            private readonly string m_Name;
            private readonly Func<TransactionCtx, Task<bool>> m_Func;

            public ProcedureInnerAsync(Func<TransactionCtx, Task<bool>> func, string name)
            {
                m_Func = func;
                m_Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            }

            public async Task<bool> Process(TransactionCtx ctx)
            {
                return await m_Func(ctx);
            }
        }
    }
}