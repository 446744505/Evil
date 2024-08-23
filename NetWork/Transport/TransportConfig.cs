using Evil.Util;

namespace NetWork.Transport
{
    public abstract class TransportConfig
    {
        public ushort Pvid { get; set; } = 0;
        public int Port { get; set; } = 27519;
        public int SoRcvbuf { get; set; } = 65536;
        public int SoSndbuf { get; set; } = 65536;
        public int OutBufferSize { get; set; } = 1048576;
        public INetWorkFactory? NetWorkFactory { get; set; }

        internal Executor Executor { get; } = new();
        public IMessageDispatcher? Dispatcher { get; set; }
        public IMessageProcessor MessageProcessor { get; set; } = null!;
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