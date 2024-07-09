using System;
using Client.NetWork;
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
                config.SessionFactory = new ClientSessionFactory();
                var connector = new ConnectorTransport(config);
                connector.Start();
                Console.WriteLine("client started");
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