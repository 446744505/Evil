
namespace Edb
{
    internal sealed class Lockey : IComparable<Lockey>
    {
        private readonly int m_Index;
        private readonly object m_Key;
        private readonly int m_HashCode;
        private volatile ReaderWriterLockSlim m_RWLock = null!;
        
        internal object Key => m_Key;
        
        internal Lockey(int id, object key)
        {
            m_Index = id;
            m_Key = key;
            m_HashCode = id ^ (id << 16) ^ key.GetHashCode();
        }

        internal Lockey Alloc()
        {
            m_RWLock = new ReaderWriterLockSlim();
            return this;
        }

        internal void RLock(int timeoutMills)
        {
            if (!m_RWLock.TryEnterUpgradeableReadLock(timeoutMills))
            {
                throw new LockTimeoutException($"Timeout waiting for read lock on {this}");
            }
        }
        
        internal void WLock(int timeoutMills)
        {
            if (!m_RWLock.TryEnterWriteLock(timeoutMills))
            {
                throw new LockTimeoutException($"Timeout waiting for write lock on {this}");
            }
        }
        
        internal void RUnlock()
        {
            m_RWLock.ExitUpgradeableReadLock();
        }
        
        internal void WUnlock()
        {
            m_RWLock.ExitWriteLock();
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