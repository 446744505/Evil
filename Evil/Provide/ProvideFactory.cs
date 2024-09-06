using NetWork;
using NetWork.Transport;
using Proto;

namespace Evil.Provide
{
    public interface IProvideFactory
    {
        ushort Pvid();
        IMessageRegister MessageRegister();
        ProvideType Type();
        /// <summary>
        /// 连接某个provider的providee有变化
        /// </summary>
        /// <param name="providerUrl"></param>
        /// <param name="news"></param>
        void OnProvideUpdate(
            string providerUrl,
            Dictionary<ushort, ProvideInfo> newAll, 
            List<ProvideInfo> added, 
            List<ProvideInfo> removed);

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