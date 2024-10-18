
using System;
using Evil.Util;

namespace NetWork
{
    public interface IMessageDispatcher
    {
        void Dispatch(Message msg);
    }
    
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly Executor m_Executor;

        public MessageDispatcher(Executor executor)
        {
            m_Executor = executor;
        }

        public void Dispatch(Message msg)
        {
            m_Executor.Execute(() =>
            {
                try
                {
                    msg.Process();
                }
                catch (Exception e)
                {
                    Log.I.Error($"process msg {msg}", e);
                }
            });
        }
    }
}