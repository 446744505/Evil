namespace Edb
{
    internal class Savepoint
    {
        private readonly Dictionary<object, ILog> m_Logs = new();
        private readonly List<ILog> m_AddOrder = new();
        private int m_Access = 0;
        
        internal int Access => m_Access;

        internal int Commit()
        {
            foreach (var log in m_AddOrder)
            {
                log.Commit();
            }

            return m_AddOrder.Count;
        }
        
        internal int Rollback()
        {
            for (var i = m_AddOrder.Count - 1; i >= 0; i--)
            {
                m_AddOrder[i].Rollback();
            }

            return m_AddOrder.Count;
        }

        internal bool IsAccessSince(int a)
        {
            return a != m_Access;
        }

        internal ILog? Get(LogKey key)
        {
            ++m_Access;
            m_Logs.TryGetValue(key, out var log);
            return log;
        }
        
        internal void Add(ILog log)
        {
            m_AddOrder.Add(log);
        }

        internal void Add(LogKey key, ILog log)
        {
            ++m_Access;
            if (m_Logs.TryGetValue(key, out _))
            {
                throw new XError("impossible:log already exists in savepoint");
            }
     
            m_Logs.Add(key, log);
            m_AddOrder.Add(log);
        }
        
        internal bool AddIfAbsent(LogKey key, ILog log)
        {
            ++m_Access;
            if (m_Logs.ContainsKey(key))
            {
                return false;
            }
            m_Logs.Add(key, log);
            m_AddOrder.Add(log);
            return true;
        }
    }
}