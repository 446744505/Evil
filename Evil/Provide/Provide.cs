
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
        
        private void CalcChangedProvides(
            Dictionary<ushort, ProvideInfo> newAll,
            Dictionary<ushort, ProvideInfo>? old,
            out List<ProvideInfo> added,
            out List<ProvideInfo> removed)
        {
            added = new();
            removed = new();
            if (old is null)
            {
                foreach (var pair in newAll)
                {
                    added.Add(pair.Value);
                }
                return;
            }
            foreach (var pair in newAll)
            {
                if (!old.ContainsKey(pair.Key))
                {
                    added.Add(pair.Value);
                }
            }
            foreach (var pair in old)
            {
                if (!newAll.ContainsKey(pair.Key))
                {
                    removed.Add(pair.Value);
                }
            }
        }
        
        private void OnProvidesUpdate(string providerUrl, string json)
        {
            var infos = JsonSerializer.Deserialize<Dictionary<ushort, ProvideInfo>>(json)!;
            if (m_Provides.TryRemove(providerUrl, out var old))
            {
                Log.I.Info($"provider {providerUrl} update provides {Strings.ToCustomString(old)} -> {Strings.ToCustomString(infos)}");
            }
                
            CalcChangedProvides(infos, old, out var added, out var removed);
            m_Provides[providerUrl] = infos;
            // 框架内部更新
            m_Sessions.OnProvideUpdate(providerUrl, infos, added, removed);
            // 框架外部自定义更新
            m_Factory.OnProvideUpdate(providerUrl, infos, added, removed);
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

            Etcd.I.Init(etcd);
            
            var kvs = await Etcd.I.GetRangeAsync(key);
            foreach (var kv in kvs)
            {
                var providerUrl = RemovePrefix(kv.Key);
                Connect(providerUrl);
                OnProvidesUpdate(providerUrl, kv.Value);
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
                    OnProvidesUpdate(providerUrl, e.Value);
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