namespace Edb
{
    internal class Tables
    {
        private readonly Dictionary<string, BaseTable> m_Tables = new();
        private readonly ReaderWriterLockSlim m_FlushLock = new();
        
        internal ReaderWriterLockSlim FlushLock => m_FlushLock;
    }
}