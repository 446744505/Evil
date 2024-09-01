using NetWork;
using NetWork.Transport;

namespace Evil.Provide
{
    public class ProvideConnectorTransport : ConnectorTransport
    {
        private readonly IMessageRegister m_InnerRegister;
        public ProvideConnectorTransport(ProvideConnectorTransportConfig config, IMessageRegister innerRegister) : base(config)
        {
            m_InnerRegister = innerRegister;
        }
        
        public override void RegisterExtMessages()
        {
            base.RegisterExtMessages();
            // 注册框架内部消息
            m_InnerRegister.Register(Config.MessageProcessor);
        }
    }
}