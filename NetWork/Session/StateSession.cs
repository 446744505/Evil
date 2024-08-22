using DotNetty.Transport.Channels;

namespace NetWork
{
    public class StateSession : Session
    {
        protected int m_State;
        
        public StateSession(IChannelHandlerContext context) : base(context)
        {
        }

        public void AddState(int state)
        {
            lock (this)
            {
                m_State |= state;   
            }
        }
        
        public void RemoveState(int state)
        {
            lock (this)
            {
                m_State &= ~state;
            }
        }
        
        public bool HasState(int state)
        {
            return (m_State & state) == state;
        }
    }
}