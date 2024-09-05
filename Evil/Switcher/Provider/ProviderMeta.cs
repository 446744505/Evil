using System.Collections.Concurrent;
using System.Text.Json;
using Evil.Util;
using Proto;

namespace Evil.Switcher
{
    public class ProviderMeta
    {
        private readonly string m_Url;
        private ConcurrentDictionary<ushort, ProvideInfo> m_Provides = new();

        public ProviderMeta(string url)
        {
            m_Url = url;
        }
        
        public void AddProvide(ProvideInfo provideInfo)
        {
            m_Provides[(ushort)provideInfo.pvid] = provideInfo;
            UpdateEtcd();
        }
        
        public  void RemoveProvide(ushort pvid)
        {
            if (m_Provides.TryRemove(pvid, out _))
            {
                UpdateEtcd();   
            }
        }

        private (string, string) ToKv()
        {
            var json = JsonSerializer.Serialize(m_Provides);
            return ($"provider/{m_Url}", json);
        }

        public void UpdateEtcd()
        {
            var (k, v) = ToKv();
            _ = Etcd.I.PutAsync(k, v);
        }
    }
}