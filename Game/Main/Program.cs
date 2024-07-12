using Game.NetWork;
using NetWork;
using NetWork.Transport;
using NetWork.Util;

namespace Game
{
    public static class Program
    {
        public static void Main(string[] args)
        {
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
}