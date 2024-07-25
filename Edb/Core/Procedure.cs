namespace Edb
{
    public interface Procedure : IProcedure
    {
        public bool Process();
        
        public void Execute()
        {
            Execute(this);
        }
        
        public IResult Call()
        {
            return Call(this);
        }
        
        public Task Submit()
        {
            return Submit(this);
        }

        static void Execute<TP>(TP p, IDone<TP>? done = null) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, done);
        }
        
        static void Execute<TP>(TP p, Action<TP,IResult> done) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, new DoneInner<TP>(done));
        }
        
        static void Execute(Func<bool> p)
        {
            Execute(new ProcedureInner(p));
        }
        
        static IResult Call<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Call(p);
        }
        
        static IResult Call(Func<bool> p)
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

        private struct DoneInner<TP> : IDone<TP> where TP : IProcedure
        {
            private readonly Action<TP, IResult> m_Done;

            public DoneInner(Action<TP, IResult> done)
            {
                m_Done = done;
            }

            public void DoDone(TP p, IResult r)
            {
                m_Done(p, r);
            }
        }
        
        private struct ProcedureInner : Procedure
        {
            private readonly Func<bool> m_Func;

            public ProcedureInner(Func<bool> func)
            {
                m_Func = func;
            }

            public bool Process()
            {
                return m_Func();
            }
        }
    }
}