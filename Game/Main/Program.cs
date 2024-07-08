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
                var acceptor = new AcceptorTransport(new AcceptorTransportConfig());
                acceptor.Start();
                Console.WriteLine("server started");
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