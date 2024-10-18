using Evil.Util;
using NetWork.Transport;
using Proto;

namespace Evil.Switcher
{
    internal class Linker : Singleton<Linker>
    {
        #region 字段

        private ITransport? m_Transport;
        private readonly LinkerSessions m_Sessions = new();

        #endregion
        
        #region 属性

        internal LinkerSessions Sessions => m_Sessions;
        internal int SessionTimeout { get; private set; }

        #endregion

        internal void Start()
        {
            SessionTimeout = TimeSpan.FromSeconds(CmdLine.I.LinkerSessionTimeout).Milliseconds;

            StartNetWork();
        }

        internal void Stop()
        {
            m_Transport?.Dispose();
        }

        private void StartNetWork()
        {
            var netConfig = new AcceptorTransportConfig();
            netConfig.Port = CmdLine.I.LinkerPort;
            netConfig.NetWorkFactory = new LinkerNetWorkFactory();
            m_Transport = new AcceptorTransport(netConfig);
            m_Transport.Start();
            Log.I.Info("linker started");
        }

        internal bool CanAddSession()
        {
            return m_Sessions.Count < CmdLine.I.MaxSessionCount;
        }

        internal void CloseSession(LinkerSession session, int code)
        {
            Log.I.Info($"close client session {session}, code {code}");
            session.Send(new SessionError { code = code });
            session.Close();
        }
    }
}