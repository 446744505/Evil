using NetWork;
using NetWork.Transport;

namespace Evil.Provide
{
    public interface IProvideFactory
    {
        ushort Pvid();
        IMessageRegister MessageRegister();
        ProvideType Type();

        ProvideNetWorkFactory CreateNetWorkFactory()
        {
            return new ProvideNetWorkFactory(MessageRegister());
        }

        IMessageDispatcher CreateMessageDispatcher(TransportConfig config)
        {
            return new MessageDispatcher(config.Executor);
        }
    }

    public enum ProvideType
    {
        _,
        Game,
        Map
    }
}