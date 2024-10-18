namespace Edb
{
    /// <summary>
    /// 执行完后可携带结果的Procedure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RProcedure<T> : Procedure
    {
        public T R { get; protected set; }
        public abstract bool Process();
    }
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

        static void Execute<TP>(TP p, Action<TP,IResult>? done = null) where TP : IProcedure
        {
            ProcedureImpl<TP>.Execute(p, done);
        }
        
        static void Execute(Func<bool> p, string name = "")
        {
            Execute(new ProcedureInner(p, name));
        }
        
        static IResult Call<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Call(p);
        }
        
        static IResult Call(Func<bool> p, string name = "")
        {
            return Call(new ProcedureInner(p, name));
        }

        static Task<IResult> Submit<TP>(TP p) where TP : IProcedure
        {
            return ProcedureImpl<TP>.Submit(p);
        }
        
        static Task<IResult> Submit(Func<bool> p, string name = "")
        {
            return Submit(new ProcedureInner(p, name));
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

            public bool Process()
            {
                return m_Func();
            }
        }
        
        private struct ProcedureInnerAsync : Procedure
        {
            private readonly string m_Name;
            private readonly Func<bool> m_Func;

            public ProcedureInnerAsync(Func<bool> func, string name)
            {
                m_Func = func;
                m_Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            }

            public bool Process()
            {
                return m_Func();
            }
        }
    }
}