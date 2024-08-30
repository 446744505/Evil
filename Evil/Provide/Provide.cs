using NetWork;
using Proto;

namespace Evil.Provide
{
    public partial class Provide : IDisposable
    {
        #region 字段

        private readonly ProvideConnectorTransport m_Transport;

        #endregion

        #region 属性


        #endregion

        static Provide()
        {
            MessageIgnore.Init();
        }

        public Provide(ProvideConnectorTransportConfig config, IMessageRegister? register = null)
        {
            if (config.NetWorkFactory == null)
            {
                if (register == null)
                    throw  new NetWorkException("provide register is null");
                
                config.NetWorkFactory = new ProvideNetWorkFactory(register);
            }
            m_Transport = new ProvideConnectorTransport(config, new MessageRegister());
        }

        public void Start()
        {
            m_Transport.Start();
        }

        public void Dispose()
        {
            m_Transport.Dispose();
        }
    }
}