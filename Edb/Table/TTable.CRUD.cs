using Evil.Util;

namespace Edb
{
    public abstract partial class TTable<TKey, TValue> : BaseTable
        where TKey : notnull where TValue : class
    {
        protected async Task<bool> AddAsync(TKey key, TValue value)
        {
            if (value == null)
                throw new NullReferenceException("value is null");

            var lockey = Lockeys.GetLockey(m_LockId, key);
            Transaction.Current!.WAddLockey(lockey);
            Interlocked.Decrement(ref m_CountAdd);
            var r = Cache.Get(key);
            if (r != null)
                return r.Add(value);
            
            Interlocked.Increment(ref m_CountAddMiss);
            if (await Exist0Async(key))
            {
                Interlocked.Increment(ref m_CountAddStorageMiss);
                return false;
            }
            Cache.Add(key, new TRecord<TKey, TValue>(this, value, lockey, TRecord<TKey, TValue>.State.Add));
            
            return true;
        }

        protected async Task<bool> RemoveAsync(TKey key)
        {
            var transaction = Transaction.Current;
            var lockey = Lockeys.GetLockey(m_LockId, key);
            transaction!.WAddLockey(lockey);
            transaction.RemoveCacheTRecord(this, key);
            Interlocked.Increment(ref m_CountRemove);
            var r = Cache.Get(key);
            if (r != null)
                return r.Remove();
            
            Interlocked.Increment(ref m_CountRemoveMiss);
            var exist = await Exist0Async(key);
            if (!exist)
                Interlocked.Increment(ref m_CountRemoveStorageMiss);
            Cache.Add(key, new TRecord<TKey, TValue>(this, null, lockey, exist ? TRecord<TKey, TValue>.State.InDbRemove : TRecord<TKey, TValue>.State.Remove));
            return exist;
        }

        protected async Task<TValue?> GetAsync(TKey key, bool wLock)
        {
            var transaction = Transaction.Current;
            var lockey = Lockeys.GetLockey(m_LockId, key);
            if (wLock)
                await transaction!.WAddLockey(lockey);
            else
                await transaction!.RAddLockey(lockey);
            Interlocked.Increment(ref m_CountGet);
            var rCached = transaction.GetCacheTRecord(this, key);
            if (rCached != null)
                return rCached.Value;
            var cacheLockey = Lockeys.GetLockey(-m_LockId, key);
            var release = await cacheLockey.WLock(Edb.I.Config.LockTimeoutMills);
            try
            {
                var r = Cache.Get(key);
                if (r == null)
                {
                    Interlocked.Increment(ref m_CountGetMiss);
                    var value = await Find0Async(key);
                    if (value == null)
                    {
                        Interlocked.Increment(ref m_CountGetStorageMiss);
                        return null;
                    }
                    r = new TRecord<TKey, TValue>(this, value, lockey, TRecord<TKey, TValue>.State.InDbGet);
                    Cache.AddNoLog(key, r);
                }
                transaction.AddCacheTRecord(this, r);
                return r.Value;
            }
            finally
            {
                cacheLockey.WUnlock(release);
            }
        }

        public Task WalkAsync(Action<TValue> cb)
        {
            if (Storage == null)
                throw new NotSupportedException($"walk on memory table {m_Config.Name}, use TTableCache.Walk instead");
            return Storage.Engine.WalkAsync(doc =>
            {
                try
                {
                    var value = UnmarshalValue(doc);
                    cb(value);
                } catch (Exception e)
                {
                    Log.I.Error(e);
                }
            });
        }
        
        private Task<bool> Exist0Async(TKey key)
        {
            return Storage == null ? Task.FromResult(false) : Storage.Exist(key, this);
        }

        private Task<TValue?> Find0Async(TKey key)
        {
            return Storage == null ? Task.FromResult<TValue?>(null) : Storage.Find(key, this);
        }
    }
}