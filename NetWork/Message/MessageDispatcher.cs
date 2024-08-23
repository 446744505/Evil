using System.Threading.Tasks;
using Evil.Util;

namespace NetWork
{
    public interface IMessageDispatcher
    {
        Task<bool> Dispatch(Message msg);
    }
    
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly Executor m_Executor;

        public MessageDispatcher(Executor executor)
        {
            m_Executor = executor;
        }

        public async Task<bool> Dispatch(Message msg)
        {
            return await m_Executor.ExecuteAsync(msg.Process);
        }
    }
}