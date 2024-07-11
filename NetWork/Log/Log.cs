using NetWork.Util;
using NLog;

namespace NetWork
{
    public class Log : Singleton<Log>
    {
        private readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        public void Info(string log)
        {
            m_Logger.Info(log);
        }
    }
}