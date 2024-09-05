
using System.Collections.Concurrent;
using System.Text.Json;
using Evil.Util;
using Proto;

namespace Evil.Provide
{
    public partial class Provide : IDisposable
    {
        #region 字段

        private IProvideFactory m_Factory;
        private readonly ProvideSessions m_Sessions;
        private readonly ConcurrentDictionary<string, ProvideConnectorTransport> m_Transports = new();
        private readonly ConcurrentDictionary<string, Dictionary<ushort, ProvideInfo>> m_Provides = new();

        #endregion

        #region 属性

        public ushort Pvid { get; set; }
        public ProvideType Type { get; set; }
        internal ProvideSessions Sessions => m_Sessions;

        #endregion

        static Provide()
        {
            MessageIgnore.Init();
        }

        public Provide(IProvideFactory factory)
        {
            Pvid = factory.Pvid();
            Type = factory.Type();
            m_Factory = factory;
            m_Sessions = new ProvideSessions(this);
        }

        public async Task Start(string etcd)
        {
            const string key = "provider/";
            var innerMsgRegister = new MessageRegister();

            string RemovePrefix(string k)
            {
                return k.Substring(key.Length);
            }
            
            void Connect(string providerUrl)
            {
                var hostPort = providerUrl.Split(":");
                var config = new ProvideConnectorTransportConfig(this);
                config.Host = hostPort[0];
                config.Port = Convert.ToInt32(hostPort[1]);
                config.Dispatcher = m_Factory.CreateMessageDispatcher(config);
                config.NetWorkFactory = m_Factory.CreateNetWorkFactory();
                var transport = new ProvideConnectorTransport(config, innerMsgRegister);
                m_Transports[providerUrl] = transport;
                transport.Start();
            }

            void AddOrUpdateProvide(string providerUrl, string json)
            {
                var infos = JsonSerializer.Deserialize<Dictionary<ushort, ProvideInfo>>(json)!;
                if (m_Provides.TryRemove(providerUrl, out var old))
                {
                    Log.I.Info($"provider {providerUrl} update provides {Strings.ToCustomString(old)} -> {Strings.ToCustomString(infos)}");
                }
                
                m_Provides[providerUrl] = infos;
                // 框架内部更新
                m_Sessions.OnProvideUpdate(providerUrl, infos);
                // 框架外部自定义更新
                m_Factory.OnProvideUpdate(providerUrl, infos);
            }

            Etcd.I.Init(etcd);
            
            var kvs = await Etcd.I.GetRangeAsync(key);
            foreach (var kv in kvs)
            {
                var providerUrl = RemovePrefix(kv.Key);
                Connect(providerUrl);
                AddOrUpdateProvide(providerUrl, kv.Value);
            }
            _ = Etcd.I.WatchRangeAsync(key, events =>
            {
                // 不考虑删除provider
                var enumerable = events.Where(e => e.Type == Mvccpb.Event.Types.EventType.Put);
                foreach (var e in enumerable)
                {
                    var providerUrl = RemovePrefix(e.Key);
                    if (!m_Transports.ContainsKey(providerUrl))
                    {
                        Connect(providerUrl);
                    }
                    AddOrUpdateProvide(providerUrl, e.Value);
                }
            }); 
        }

        public void Dispose()
        {
            foreach (var transport in m_Transports)
            {
                transport.Value.Dispose();
            }
        }
    }
}