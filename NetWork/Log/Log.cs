using System;
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

        public void Error(Exception e)
        {
            m_Logger.Error($"{e.Message}{e.StackTrace}");
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
        
        public void Fatal(string log)
        {
            m_Logger.Fatal(log);
        }

        public void Trace(string log)
        {
            m_Logger.Trace(log);
        }
    }
}