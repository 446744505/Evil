
using Evil.Util;

namespace Edb
{
    internal sealed class Lockey : IComparable<Lockey>
    {
        private readonly int m_Index;
        private readonly object m_Key;
        private readonly int m_HashCode;
        private volatile LockAsync m_RWLock = null!;
        
        internal object Key => m_Key;
        
        internal Lockey(int id, object key)
        {
            m_Index = id;
            m_Key = key;
            m_HashCode = id ^ (id << 16) ^ key.GetHashCode();
        }

        internal Lockey Alloc()
        {
            m_RWLock = new LockAsync();
            return this;
        }
        
        internal async Task<bool> RTryLock()
        {
            // 用1ms模拟尝试获取锁
            try {
                await RLock(1);
                return true;
            } catch (LockTimeoutException) {
                return false;
            }
        }
        
        internal async Task RLock(int timeoutMills)
        {
            await m_RWLock.RLockAsync(timeoutMills);
        }
        
        internal async Task RLock()
        {
            await m_RWLock.RLockAsync();
        }
        
        internal void RUnlock()
        {
            m_RWLock.RUnlock();
        }

        internal async Task<bool> WTryLock()
        {
            // 用1ms模拟尝试获取锁
            try {
                await WLock(1);
                return true;
            } catch (LockTimeoutException) {
                return false;
            }
        }
        
        internal async Task WLock(int timeoutMills)
        {
            await m_RWLock.WLockAsync(timeoutMills);
        }

        internal void WUnlock()
        {
            m_RWLock.WUnlock();
        }
        
        public int CompareTo(Lockey? other)
        {
            var x = m_Index - other!.m_Index;
            return x != 0 ? x : ((IComparable)m_Key).CompareTo(other.m_Key);
        }
        
        public override int GetHashCode()
        {
            return m_HashCode;
        }
        
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj is Lockey other)
                return m_Index == other.m_Index && m_Key.Equals(other.m_Key);
            return false;
        }
        
        public override string ToString()
        {
            return $"{m_Index}.{m_Key}";
        }
    }
}