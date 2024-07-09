using NLog;

namespace NetWork
{
    public class Log
    {
        public static Log I { get; } = new();
        
        private readonly Logger m_Logger;
        private Log()
        {
            m_Logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string log)
        {
            m_Logger.Info(log);
        }
    }
}