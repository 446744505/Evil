using Evil.Util;

namespace NetWork.Transport
{
    public abstract class TransportConfig
    {
        public int Port { get; set; } = 27519;
        public INetWorkFactory NetWorkFactory { get; set; } = null!;

        internal Executor Executor { get; } = new();
        public IMessgeDispatcher Dispatcher { get; set; } = null!;
    }
    
    public class AcceptorTransportConfig : TransportConfig
    {
       public int Backlog { get; set; } = 32;
    }
    
    public class ConnectorTransportConfig : TransportConfig
    {
        public string Host { get; set; } = "127.0.0.1";
        public int WorkerCount { get; set; } = 1;
    }
}