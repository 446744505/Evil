using System.Threading.Tasks;
using Evil.Util;

namespace NetWork
{
    public interface IMessgeDispatcher
    {
        Task<bool> Dispatch(Message msg);
    }
    
    public class MessgeDispatcher : IMessgeDispatcher
    {
        private readonly Executor m_Executor;

        public MessgeDispatcher(Executor executor)
        {
            m_Executor = executor;
        }

        public async Task<bool> Dispatch(Message msg)
        {
            return await m_Executor.ExecuteAsync(msg.Process);
        }
    }
}