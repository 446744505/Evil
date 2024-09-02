using System.Threading.Tasks;
using Evil.Util;

namespace NetWork
{
    public interface IMessageDispatcher
    {
        Task Dispatch(Message msg);
    }
    
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly Executor m_Executor;

        public MessageDispatcher(Executor executor)
        {
            m_Executor = executor;
        }

        public Task Dispatch(Message msg)
        {
            return m_Executor.ExecuteAsync(msg.Process);
        }
    }
}