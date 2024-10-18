using Evil.Util;

namespace Edb
{
    public partial class Transaction
    {
        private readonly Dictionary<Lockey, LockeyHolder> m_Locks = new();
        
        internal LockeyHolderType GetLockeyHolderType(Lockey lockey)
        {
            return m_Locks.TryGetValue(lockey, out var holder) ? holder.m_Type : LockeyHolderType.None;
        }

        internal Lockey? Get(Lockey lockey)
        {
            if (m_Locks.TryGetValue(lockey, out var holder))
            {
                return holder.m_Lockey;
            }

            return null;
        }

        internal void RAddLockey(Lockey lockey)
        {
            if (m_Locks.ContainsKey(lockey))
                return;
            lockey.RLock(Edb.I.Config.LockTimeoutMills);
            m_Locks[lockey] = new LockeyHolder(lockey, LockeyHolderType.Read);
        }

        internal void WAddLockey(Lockey lockey)
        {
            if (!m_Locks.TryGetValue(lockey, out var holder))
            {
                lockey.WLock(Edb.I.Config.LockTimeoutMills);
                m_Locks[lockey] = new LockeyHolder(lockey, LockeyHolderType.Write);
            } else if (holder.m_Type == LockeyHolderType.Read)
            {
                holder.m_Lockey.RUnlock();
                try
                {
                    holder.m_Lockey.WLock(Edb.I.Config.LockTimeoutMills);
                }
                catch (LockTimeoutException)
                {
                    m_Locks.Remove(lockey);
                    throw;
                }
                holder.m_Type = LockeyHolderType.Write;
            }
        }
        
        internal enum LockeyHolderType
        {
            Write,
            Read,
            None
        }
        
        private class LockeyHolder : IComparable<LockeyHolder>
        {
            internal readonly Lockey m_Lockey;
            internal LockeyHolderType m_Type;

            internal LockeyHolder(Lockey lockey, LockeyHolderType type)
            {
                m_Lockey = lockey;
                m_Type = type;
            }

            internal void Cleanup()
            {
                if (m_Type == LockeyHolderType.Write)
                    m_Lockey.WUnlock();
                else
                    m_Lockey.RUnlock();
            }

            public int CompareTo(LockeyHolder? other)
            {
                var x = m_Lockey.CompareTo(other!.m_Lockey);
                return x != 0 ? x : m_Type.CompareTo(other.m_Type);
            }
        }
    }
}