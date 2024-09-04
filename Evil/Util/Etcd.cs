using System.Collections.Concurrent;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;

namespace Evil.Util
{
    public class Etcd : Singleton<Etcd>, IDisposable
    {
        private EtcdClient m_Client = null!;
        private long m_LeaseId; // 当前的租约id
        private CancellationTokenSource m_LeaseCancelCts = null!; // 维护租约任务的取消ctx
        private ConcurrentDictionary<string, string> m_Metas = new();

        private const int LeaseTtl = 10; // 租约的过期时间，单位秒
        private static readonly TimeSpan LeaseReTryInterval = TimeSpan.FromSeconds(5);

        public void Init(string url)
        {
            m_Client = new EtcdClient(url);
            
            m_LeaseCancelCts = new CancellationTokenSource();
            Task.Run(MaintainLeaseAsync, m_LeaseCancelCts.Token);
        }

        public async Task PutAsync(string key, string value)
        {
            await PutAsync(key, value, m_LeaseCancelCts.Token);
        }
        
        public async Task PutAsync(string key, string value, CancellationToken cancellationToken)
        {
            m_Metas[key] = value;
            if (m_LeaseId == 0)
                return;
            
            var req = new PutRequest
            {
                Lease = m_LeaseId,
                Key = ByteString.CopyFromUtf8(key),
                Value = ByteString.CopyFromUtf8(value)
            };
            await m_Client.PutAsync(req, cancellationToken: cancellationToken);
            Log.I.Info($"etcd put key:{key} val:{value} success");
        }

        public async Task Delete(string key)
        {
            m_Metas.Remove(key, out _);
            await m_Client.DeleteAsync(key);
        }
        
        public async Task<string> GetAsync(string key)
        {
            var resp = await m_Client.GetAsync(key);
            return resp.Kvs.Count > 0 ? resp.Kvs[0].Value.ToStringUtf8() : "";
        }
        
        public async Task<bool> ExistAsync(string key)
        {
            var resp = await m_Client.GetAsync(key);
            return resp.Kvs.Count > 0;
        }
        
        public async Task WatchRangeAsync(string key, Action<WatchEvent[]> cb)
        {
            await m_Client.WatchRangeAsync(key, cb);
        }
        
        public async Task WatchAsync(string key, Action<WatchEvent[]> cb)
        {
            await m_Client.WatchAsync(key, cb);
        } 

        public async Task<Dictionary<string, string>> GetRangeAsync(string key)
        {
            var ret = new Dictionary<string, string>();
            var resp = await m_Client.GetRangeAsync(key);
            foreach (var kv in resp.Kvs)
            {
                ret[kv.Key.ToStringUtf8()] = kv.Value.ToStringUtf8();
            }

            return ret;
        }
        
        private async Task PutAllAsync(CancellationToken cancellationToken)
        {
            foreach (var pair in m_Metas)
            {
                await PutAsync(pair.Key, pair.Value, cancellationToken);
            }
        }

        private async Task<long> CreateLeaseAsync(CancellationToken cancellationToken)
        {
            // 创建一个新的租约并返回租约ID
            var leaseGrantResponse = await m_Client.LeaseGrantAsync(new LeaseGrantRequest
            {
                TTL = LeaseTtl // 确保在这里设置了你想要的租约TTL值
            }, cancellationToken: cancellationToken);
            return leaseGrantResponse.ID;
        }
        
        private async Task MaintainPutWithLease(CancellationToken cancellationToken)
        {
            // Step 1: 创建租约并获取租约ID
            m_LeaseId = await CreateLeaseAsync(cancellationToken);
            Log.I.Info($"etcd create lease success, leaseId:{m_LeaseId}");

            // Step 2: 使用租约ID执行put操作
            await PutAllAsync(cancellationToken);

            // Step 3: 启动租约保活循环
            await m_Client.LeaseKeepAlive(m_LeaseId, cancellationToken);
        }

        private async Task MaintainLeaseAsync()
        {
            while (!m_LeaseCancelCts.IsCancellationRequested)
            {
                try
                {
                    await MaintainPutWithLease(m_LeaseCancelCts.Token);
                }
                catch (OperationCanceledException)
                {
                    // 任务被取消时会抛出这个异常，可以在这里进行清理工作
                    if (m_LeaseId != 0)
                    {
                        await m_Client.LeaseRevokeAsync(new LeaseRevokeRequest
                        {
                            ID = m_LeaseId
                        });
                        Log.I.Info($"clear leaseId:{m_LeaseId} success");
                    }
                }
                catch (Exception e)
                {
                    Log.I.Error($"lease keep alive failed,retry after {LeaseReTryInterval}", e);
                    // 重置租约ID
                    m_LeaseId = 0;
                    // 等待一段时间重试
                    await Task.Delay(LeaseReTryInterval, m_LeaseCancelCts.Token);
                }
            }
        }
        
        public void Dispose()
        {
            Log.I.Info("etcd start stop");
            // 取消维护租约的任务
            m_LeaseCancelCts.Cancel();
            
            m_Client.Dispose();
            Log.I.Info("etcd stop success");
        }
    }
}