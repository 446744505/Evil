using Evil.Provide;
using Evil.Util;
using Map.NetWork;
using NetWork;
using NetWork.Transport;
using Proto;

namespace Map
{
    public class MapProvideFactory : IProvideFactory
    {
        public ushort Pvid()
        {
            return CmdLine.I.Pvid;
        }
        
        public IMessageRegister MessageRegister()
        {
            return Net.I.MessageRegister;
        }

        public ProvideType Type()
        {
            return ProvideType.Map;
        }

        public void OnProvideUpdate(string providerUrl, Dictionary<ushort, ProvideInfo> newAll)
        {
            
        }

        public IMessageDispatcher CreateMessageDispatcher(TransportConfig _)
        {
            return new ProcedureHelper.MessageDispatcher();
        }
    }
}