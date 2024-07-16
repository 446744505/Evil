namespace Edb
{
    public class Transaction
    {
        #region 变量

        private readonly List<Savepoint> m_Savepoints = new();
        private readonly Dictionary<Lockey, LockeyHolder> m_Locks = new();

        #endregion

        #region 属性

        #endregion

        #region static变量

        private static readonly ThreadLocal<Transaction> ThreadLocal = new();

        #endregion

        #region static属性

        internal static Transaction? Current => ThreadLocal.Value;
        internal static Savepoint CurrentSavepoint => Current!.m_Savepoints[^1];

        #endregion

        internal LockeyHolderType GetLockeyHolderType(Lockey lockey)
        {
            return m_Locks.TryGetValue(lockey, out var holder) ? holder.m_Type : LockeyHolderType.None;
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