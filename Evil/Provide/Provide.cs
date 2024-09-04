
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
        private readonly ConcurrentDictionary<string, ProvideConnectorTransport> m_Transports = new();
        private readonly ConcurrentDictionary<string, List<ProvideInfo>> m_Provides = new();

        #endregion

        #region 属性

        public ushort Pvid { get; set; }
        public ProvideType Type { get; set; }

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
        }

        public async Task Start(string etcd)
        {
            const string key = "provider/";
            var innerMsgRegister = new MessageRegister();

            string RemovePrefix(string k)
            {
                return k.Substring(key.Length);
            }
            
            void Connect(string url)
            {
                var hostPort = url.Split(":");
                var config = new ProvideConnectorTransportConfig(this);
                config.Host = hostPort[0];
                config.Port = Convert.ToInt32(hostPort[1]);
                config.Dispatcher = m_Factory.CreateMessageDispatcher(config);
                config.NetWorkFactory = m_Factory.CreateNetWorkFactory();
                var transport = new ProvideConnectorTransport(config, innerMsgRegister);
                m_Transports[url] = transport;
                transport.Start();
            }

            void AddOrUpdateProvide(string url, string json)
            {
                var info = JsonSerializer.Deserialize<List<ProvideInfo>>(json);
                if (m_Provides.TryRemove(url, out var old))
                {
                    Log.I.Info($"provider {url} update provides {Strings.ToCustomString(old)} -> {Strings.ToCustomString(info)}");
                }
                m_Provides[url] = info!;
            }

            Etcd.I.Init(etcd);
            
            var kvs = await Etcd.I.GetRangeAsync(key);
            foreach (var kv in kvs)
            {
                var url = RemovePrefix(kv.Key);
                Connect(url);
                AddOrUpdateProvide(url, kv.Value);
            }
            _ = Etcd.I.WatchRangeAsync(key, events =>
            {
                // 不考虑删除provider
                var enumerable = events.Where(e => e.Type == Mvccpb.Event.Types.EventType.Put);
                foreach (var e in enumerable)
                {
                    var url = RemovePrefix(e.Key);
                    if (!m_Transports.ContainsKey(url))
                    {
                        Connect(url);
                    }
                    AddOrUpdateProvide(url, e.Value);
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