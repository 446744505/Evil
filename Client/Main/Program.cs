
using System.Threading.Tasks;
using Client.NetWork;
using Evil.Util;
using NetWork.Transport;

namespace Client
{
    public static class Program
    {
        public static Executor Executor { get; } = new();
        public static void Main(string[] args)
        {
            Stopper? stopper = null;
            try
            {
                var config = new ConnectorTransportConfig();
                config.Port = 10000;
                config.ReConnectDelay = 0; // 不重连
                config.NetWorkFactory = new ClientNetWorkFactory();
                var connector = new ConnectorTransport(config);
                connector.Start();
                Log.I.Info("client started");
                
                stopper = new Stopper().BindAndWait();
                
                connector.Dispose();
                Executor.Dispose();
            } finally
            {
                stopper?.SignalWeakUp();
                Log.I.Info("client stop");
            }
        }
    }
}