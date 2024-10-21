namespace Edb
{
    public partial class Transaction
    {
        private readonly List<Savepoint> m_Savepoints = new();
        
        internal int CurrentSavepointId => m_Savepoints.Count;
        
        internal Savepoint CurrentSavepoint => m_Savepoints[^1];

        public int Savepoint => Savepoint0();

        internal Savepoint? GetSavepoint(int savepoint)
        {
            if (savepoint < 1 || savepoint > m_Savepoints.Count)
                return null;
            return m_Savepoints[savepoint - 1];
        }
        
        private int Savepoint0()
        {
            m_Savepoints.Add(new Savepoint());
            return m_Savepoints.Count;
        }
    }
}