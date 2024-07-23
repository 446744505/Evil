using NLog;

namespace Evil.Util
{
    public class Log : Singleton<Log>
    {
        private readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        public void Info(string log)
        {
            m_Logger.Info(log);
        }

        public void Error(Exception e)
        {
            m_Logger.Error($"{e.Message}{e.StackTrace}");
            if (e.InnerException != null)
            {
                Error(e.InnerException);
            }
        }

        public void Error(string log)
        {
            m_Logger.Error(log);
        }
        
        public void Debug(string log)
        {
            m_Logger.Debug(log);
        }
        
        public void Warn(string log)
        {
            m_Logger.Warn(log);
        }

        public void Warn(Exception e)
        {
            m_Logger.Warn($"{e.Message}{e.StackTrace}");
            if (e.InnerException != null)
            {
                Warn(e.InnerException);
            }
        }
        
        public void Fatal(string log)
        {
            m_Logger.Fatal(log);
        }

        public void Fatal(Exception e)
        {
            m_Logger.Fatal($"{e.Message}{e.StackTrace}");
            if (e.InnerException != null)
            {
                Fatal(e.InnerException);
            }
        }

        public void Trace(string log)
        {
            m_Logger.Trace(log);
        }
    }
}