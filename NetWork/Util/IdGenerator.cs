using System.Threading;

namespace NetWork.Util
{
    public class IdGenerator
    {
        private static long m_Id = 0;
        
        public static long NextId()
        {
            return Interlocked.Increment(ref m_Id);
        }
    }
}