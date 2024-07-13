using System;
using System.Threading.Tasks;
using Client.NetWork;
using Logic.Hero.Proto;
using NetWork;
using NetWork.Transport;
using NetWork.Util;

namespace Client
{
    public static class Program
    {
        public static void Main(string[] args)
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
            } finally
            {
                stopper?.SignalWeakUp();
            }
        }
    }
}