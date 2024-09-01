using NetWork;
using Proto;

namespace Evil.Provide
{
    public partial class Provide : IDisposable
    {
        #region 字段

        private readonly List<ProvideConnectorTransport> m_Transports = new();

        #endregion

        #region 属性


        #endregion

        static Provide()
        {
            MessageIgnore.Init();
        }

        public Provide(List<ProvideConnectorTransportConfig> configs, IMessageRegister? register = null)
        {
            var innerMsgRegister = new MessageRegister();
            foreach (var config in configs)
            {
                if (config.NetWorkFactory == null)
                {
                    if (register == null)
                        throw  new NetWorkException("provide register is null");
                
                    config.NetWorkFactory = new ProvideNetWorkFactory(register);
                }
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