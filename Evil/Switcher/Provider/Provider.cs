using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork.Transport;
using Proto;

namespace Evil.Switcher
{
    internal class Provider : Singleton<Provider>
    {
        #region 字段

        private ITransport? m_Transport;
        private ProviderSessions m_Sessions = new();
        private ProviderMeta m_Meta = null!;

        #endregion

        #region 属性

        internal int SessionTimeout { get; private set; }
        internal ProviderSessions Sessions => m_Sessions;
        internal ProviderMeta Meta => m_Meta;

        #endregion

        internal void Start()
        {
            SessionTimeout = TimeSpan.FromSeconds(CmdLine.I.ProviderSessionTimeout).Milliseconds;
            InitMeta();
            
            StartNetWork();
        }

        private void InitMeta()
        {
            var address = IpAddr.GetLocalIpv4();
            if (address is null)
            {
                throw new Exception("get local ipv4 failed");
            }
            var url = $"{address.ToString()}:{CmdLine.I.ProviderPort}";
            m_Meta = new ProviderMeta(url);
        }

        internal void Stop()
        {
            m_Transport?.Dispose();
        }
        
        private void StartNetWork()
        {
            var netConfig = new AcceptorTransportConfig();
            netConfig.Port = CmdLine.I.ProviderPort;
            netConfig.NetWorkFactory = new ProviderNetWorkFactory();
            var acceptor = new AcceptorTransport(netConfig);
            acceptor.OnStarted += RegisterToEtcd;
            acceptor.Start();
            m_Transport = acceptor;
            Log.I.Info("provider started");
        }

        private void RegisterToEtcd(object? _, IChannel channel)
        {
            m_Meta.UpdateEtcd();
        }

        internal void ClientBroken(LinkerSession session)
        {
            foreach (var pvid in session.BindProvides)
            {
                var providerSession = Sessions.GetSession(pvid);
                if (providerSession is not null)
                {
                    providerSession.Send(new ClientBroken() { clientSessionId = session.Id });
                }
            }
        }
    }
}