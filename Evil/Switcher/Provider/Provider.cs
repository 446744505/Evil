using Evil.Util;
using NetWork.Transport;

namespace Evil.Switcher
{
    internal class Provider : Singleton<Provider>
    {
        #region 字段

        private ProviderSessions m_Sessions = new();

        #endregion

        #region 属性

        internal int SessionTimeout { get; private set; }
        internal ProviderSessions Sessions => m_Sessions;

        #endregion

        internal void Start()
        {
            SessionTimeout = TimeSpan.FromSeconds(CmdLine.I.ProviderSessionTimeout).Milliseconds;
            
            StartNetWork();
        }
        
        private void StartNetWork()
        {
            var netConfig = new AcceptorTransportConfig();
            netConfig.Port = CmdLine.I.ProviderPort;
            netConfig.NetWorkFactory = new ProviderNetWorkFactory();
            var acceptor = new AcceptorTransport(netConfig);
            acceptor.Start();
            Log.I.Info("provider started");
        }
    }
}