using Evil.Util;
using Game.NetWork;
using NetWork.Transport;

namespace Game
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var parent = new TRecord<double, double>();
            var child = new Derived { m_Parent = parent };
            var record = child.GetRecord() as TRecord<object,object>;
            record?.DoSomething();
            
            Stopper? stopper = null;
            try
            {
                var config = new AcceptorTransportConfig();
                config.NetWorkFactory = new GameNetWorkFactory();
                var acceptor = new AcceptorTransport(config);
                acceptor.Start();
                Log.I.Info("server started");
                stopper = new Stopper()
                    .BindSignal()
                    .BindCancelKey()
                    .Wait();
                acceptor.Dispose();
            }
            finally
            {
                stopper?.SignalWeakUp();
            }
        }
    }
    
    public class TRecord<T1, T2>
    {
        public void DoSomething()
        {
            Log.I.Info("do something");
        }
    }

    public class Derived
    {
        public object? m_Parent { get; set; }

        public object? GetRecord()
        {
            if (m_Parent.GetType().IsGenericType && m_Parent.GetType().GetGenericTypeDefinition() == typeof(TRecord<,>))
                return m_Parent;
            return null;
        }
    }
}