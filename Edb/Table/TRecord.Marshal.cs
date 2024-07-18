namespace Edb
{
    internal partial class TRecord<TKey, TValue> where TKey : notnull
    {
        private TKey m_SnapshotKey;
        private object m_SnapshotValue;
        
        private TKey MarshalKey()
        {
            return m_Table.MarshalKey(Key);
        }
        
        private object MarshalValue()
        {
            return m_Table.MarshalValue(m_Value);
        }

        internal void Marshal0()
        {
            switch (m_State)
            {
                case State.Add:
                case State.InDbGet:
                case State.InDbAdd:
                    m_SnapshotKey = MarshalKey();
                    m_SnapshotValue = MarshalValue();
                    break;
                case State.InDbRemove:
                    m_SnapshotKey = MarshalKey();
                    break;
                case State.Remove: break;
            }
        }

        internal async Task<bool> Flush(TStorage<TKey, TValue> storage)
        {
            return false;
        }
    }
}