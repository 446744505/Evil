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
                var connector = new ConnectorTransport(new ConnectorTransportConfig());
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