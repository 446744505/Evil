using NetWork;
using NetWork.Transport;

namespace Evil.Provide
{
    public class ProvideConnectorTransport : ConnectorTransport
    {
        private readonly IMessageRegister m_Register;
        public ProvideConnectorTransport(ProvideConnectorTransportConfig config, IMessageRegister register) : base(config)
        {
            m_Register = register;
        }
        
        public override void RegisterExtMessages()
        {
            base.RegisterExtMessages();
            // 注册框架内部消息
            m_Register.Register(Config.MessageProcessor);
        }
    }
}