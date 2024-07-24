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

        static void Execute<TP>(TP p) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, null);
        }
        
        static void Execute<TP>(TP p, IDone<TP>? done) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, done);
        }
        
        static void Execute<TP>(TP p, Action<TP,IResult> done) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, new Done<TP>(done));
        }
        
        static IResult Call<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Call(p);
        }
        
        static Task<IResult> Submit<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Submit(p);
        }

        private struct Done<TP> : IDone<TP> where TP : IProcedure
        {
            private readonly Action<TP, IResult> m_Done;

            public Done(Action<TP, IResult> done)
            {
                m_Done = done;
            }

            public void DoDone(TP p, IResult r)
            {
                m_Done(p, r);
            }
        }
    }
}