using NetWork.Transport;

namespace Evil.Provide
{
    public class ProvideConnectorTransportConfig : ConnectorTransportConfig
    {
        private readonly Provide m_Provide;
        public Provide Provide => m_Provide;
        
        public ProvideConnectorTransportConfig(Provide provide)
        {
            m_Provide = provide;
        }
    }
}