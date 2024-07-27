namespace Edb
{
    public interface Procedure : IProcedure
    {
        public Task<bool> Process();
        
        public void Execute()
        {
            Execute(this);
        }
        
        public Task<IResult> Call()
        {
            return Call(this);
        }
        
        public Task Submit()
        {
            return Submit(this);
        }

        static void Execute<TP>(TP p, Action<TP,IResult>? done = null) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, done);
        }
        
        static void Execute(Func<bool> p)
        {
            Execute(new ProcedureInner(p));
        }
        
        static Task<IResult> Call<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Call(p);
        }
        
        static Task<IResult> Call(Func<bool> p)
        {
            return Call(new ProcedureInner(p));
        }
        
        static Task<IResult> Submit<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Submit(p);
        }
        
        static Task<IResult> Submit(Func<bool> p)
        {
            return Submit(new ProcedureInner(p));
        }
        
        static Task<IResult> Submit(Func<Task<bool>> p)
        {
            return Submit(new ProcedureInnerAsync(p));
        }

        private struct ProcedureInner : Procedure
        {
            private readonly Func<bool> m_Func;

            public ProcedureInner(Func<bool> func)
            {
                m_Func = func;
            }

            public Task<bool> Process()
            {
                return Task.FromResult(m_Func());
            }
        }
        
        private struct ProcedureInnerAsync : Procedure
        {
            private readonly Func<Task<bool>> m_Func;

            public ProcedureInnerAsync(Func<Task<bool>> func)
            {
                m_Func = func;
            }

            public async Task<bool> Process()
            {
                return await m_Func();
            }
        }
    }
}