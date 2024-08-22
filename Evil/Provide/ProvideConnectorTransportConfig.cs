using NetWork.Transport;

namespace Evil.Provide
{
    public class ProvideConnectorTransportConfig : ConnectorTransportConfig
    {
        public ProvideConnectorTransportConfig(ushort pvid)
        {
            Pvid = pvid;
        }
    }
}