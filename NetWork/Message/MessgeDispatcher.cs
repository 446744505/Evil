using System.Threading.Tasks;

namespace NetWork
{
    public interface IMessgeDispatcher
    {
        Task<bool> Dispatch(Message msg);
    }
    
    public class MessgeDispatcher : IMessgeDispatcher
    {
        public async Task<bool> Dispatch(Message msg)
        {
            return await Task.Run(msg.Process);
        }
    }
}