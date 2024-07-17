namespace Edb
{
    public sealed class Transaction
    {
        #region 变量

        private readonly List<Savepoint> m_Savepoints = new();
        private readonly Dictionary<Lockey, LockeyHolder> m_Locks = new();
        private readonly Dictionary<LogKey, object> m_Wrappers = new();

        #endregion

        #region 属性
        
        internal Dictionary<LogKey, object> Wrappers => m_Wrappers;

        #endregion

        #region static变量

        private static readonly ThreadLocal<Transaction> ThreadLocal = new();

        #endregion

        #region static属性

        internal static Transaction? Current => ThreadLocal.Value;
        internal static Savepoint CurrentSavepoint => Current!.m_Savepoints[^1];

        #endregion

        internal static Transaction Create()
        {
            var self = Current;
            if (self == null)
                ThreadLocal.Value = self = new Transaction();
            return self;
        }

        internal LockeyHolderType GetLockeyHolderType(Lockey lockey)
        {
            return m_Locks.TryGetValue(lockey, out var holder) ? holder.m_Type : LockeyHolderType.None;
        }

        public static int Savepoint()
        {
            return Current!.SavepointInternal();
        }
        
        private int SavepointInternal()
        {
            m_Savepoints.Add(new Savepoint());
            return m_Savepoints.Count;
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