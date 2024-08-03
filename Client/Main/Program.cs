
using System.Threading.Tasks;
using Client.NetWork;
using Evil.Util;
using NetWork.Transport;

namespace Client
{
    public static class Program
    {
        public static Executor Executor { get; } = new();
        public static async Task Main(string[] args)
        {
            Stopper? stopper = null;
            try
            {
                var config = new ConnectorTransportConfig();
                config.NetWorkFactory = new ClientNetWorkFactory();
                var connector = new ConnectorTransport(config);
                connector.Start();
                Log.I.Info("client started");
                
                stopper = new Stopper()
                    .BindSignal()
                    .BindCancelKey()
                    .Wait();
                
                connector.Dispose();
                await Executor.DisposeAsync();
            } finally
            {
                stopper?.SignalWeakUp();
            }
        }
    }
}