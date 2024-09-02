using Evil.Provide;
using Evil.Util;
using Game.NetWork;
using NetWork;
using NetWork.Transport;

namespace Game
{
    public class GameProvideFactory : IProvideFactory
    {
        public ushort Pvid()
        {
            return CmdLine.I.Pvid;
        }

        public Provider[] Providers()
        {
            return CmdLine.I.Providers;
        }

        public IMessageRegister MessageRegister()
        {
            return Net.I.MessageRegister;
        }

        public ProvideType Type()
        {
            return ProvideType.Game;
        }

        public IMessageDispatcher CreateMessageDispatcher(TransportConfig _)
        {
            return new ProcedureHelper.MessageDispatcher();
        }
    }
}