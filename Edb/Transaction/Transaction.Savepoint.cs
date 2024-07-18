namespace Edb
{
    public partial class Transaction
    {
        private readonly List<Savepoint> m_Savepoints = new();
        internal static Savepoint CurrentSavepoint => Current!.m_Savepoints[^1];
        
        public static int Savepoint()
        {
            return Current!.SavepointInternal();
        }
        
        private int SavepointInternal()
        {
            m_Savepoints.Add(new Savepoint());
            return m_Savepoints.Count;
        }
    }
}