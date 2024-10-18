using MongoDB.Bson;

namespace Edb
{
    internal partial class TRecord<TKey, TValue> 
        where TKey : notnull where TValue : class 
    {
        private TKey? m_SnapshotKey;
        private BsonDocument? m_SnapshotValue;
        private State? m_SnapshotState;

        internal bool TryMarshalN(Action action)
        {
            if (!m_Lockey.RTryLock())
                return false;

            try
            {
                Marshal0();
                action();
            }
            finally
            {
                m_Lockey.RUnlock();
            }

            return true;
        }
        
        private TKey MarshalKey()
        {
            return m_Table.MarshalKey(Key);
        }
        
        private BsonDocument MarshalValue()
        {
            return m_Table.MarshalValue(m_Value!);
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

        internal void Snapshot()
        {
            switch (m_SnapshotState = m_State)
            {
                case State.Add:
                case State.InDbAdd:
                    m_State = State.InDbGet;
                    break;
                case State.Remove:
                case State.InDbRemove:
                    m_Table.Cache.Remove(Key);
                    break;
                case State.InDbGet:
                    break;
            }
        }

        internal bool FlushAsync(TStorage<TKey, TValue> storage)
        {
            switch (m_SnapshotState)
            {
                case State.InDbAdd:
                case State.InDbGet:
                    storage.Engine.Replace(m_SnapshotKey!, m_SnapshotValue!);
                    return true;
                case State.Add:
                    if (!storage.Engine.Insert(m_SnapshotValue!))
                        throw new XError("insert fail");
                    return true;
                case State.InDbRemove:
                    storage.Engine.Remove(m_SnapshotKey!);
                    return true;
                case State.Remove:
                    break;
            }

            return false;
        }

        internal void Clear()
        {
            m_SnapshotKey = default;
            m_SnapshotValue = null;
            m_SnapshotState = null;
        }
    }
}