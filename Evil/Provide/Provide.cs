
using Proto;

namespace Evil.Provide
{
    public partial class Provide : IDisposable
    {
        #region 字段

        private readonly List<ProvideConnectorTransport> m_Transports = new();

        #endregion

        #region 属性

        public ushort Pvid { get; set; }
        public ProvideType Type { get; set; }

        #endregion

        static Provide()
        {
            MessageIgnore.Init();
        }

        public Provide(IProvideFactory factory)
        {
            Pvid = factory.Pvid();
            Type = factory.Type();
            
            var innerMsgRegister = new MessageRegister();
            foreach (var provider in factory.Providers())
            {
                var config = new ProvideConnectorTransportConfig(this);
                config.Host = provider.Host;
                config.Port = provider.Port;
                config.Dispatcher = factory.CreateMessageDispatcher(config);
                config.NetWorkFactory = factory.CreateNetWorkFactory();
                m_Transports.Add(new ProvideConnectorTransport(config, innerMsgRegister));
            }
        }

        public void Start()
        {
            foreach (var transport in m_Transports)
            {
                transport.Start();
            }
        }

        public void Dispose()
        {
            foreach (var transport in m_Transports)
            {
                transport.Dispose();
            }
        }
    }
}