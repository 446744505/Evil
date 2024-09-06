using System.Collections.Generic;
using Evil.Provide;
using Evil.Util;
using Game.NetWork;
using NetWork;
using NetWork.Transport;
using Proto;

namespace Game
{
    public partial class GameProvideFactory : IProvideFactory
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
            return ProvideType.Game;
        }

        public void OnProvideUpdate(
            string providerUrl,
            Dictionary<ushort, ProvideInfo> newAll, 
            List<ProvideInfo> added, 
            List<ProvideInfo> removed)
        {
            OnProvideUpdateMap(providerUrl, newAll, added, removed);
        }

        public IMessageDispatcher CreateMessageDispatcher(TransportConfig _)
        {
            return new ProcedureHelper.MessageDispatcher();
        }
    }
}