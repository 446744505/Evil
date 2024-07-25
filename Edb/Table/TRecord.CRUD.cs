using MongoDB.Bson;

namespace Edb
{
    internal partial class TRecord<TKey, TValue> : XBean
        where TKey : notnull where TValue : class
    {
        private void Remove0()
        {
            Logs.Link(m_Value, null, null);
            Transaction.CurrentSavepoint.AddIfAbsent(CreateLogKey(), new LogAddRemove<TKey, TValue>(this));
            m_Value = null;
        }

        internal bool Remove()
        {
            switch (m_State)
            {
                case State.InDbGet:
                    Remove0();
                    m_State = State.InDbRemove;
                    return true;
                case State.InDbAdd:
                    Remove0();
                    m_State = State.InDbRemove;
                    return true;
                case State.Add:
                    Remove0();
                    m_State = State.Remove;
                    return true;
                default:
                    return false;
            }
        }

        private void Add0(TValue value)
        {
            Logs.Link(value, this, RecordVarName);
            Transaction.CurrentSavepoint.AddIfAbsent(CreateLogKey(), new LogAddRemove<TKey, TValue>(this));
            m_Value = value;
        }

        internal bool Add(TValue value)
        {
            switch (m_State)
            {
                case State.InDbRemove:
                    Add0(value);
                    m_State = State.InDbAdd;
                    return true;
                case State.Remove:
                    Add0(value);
                    m_State = State.Add;
                    return true;
                default:
                    return false;
            }
        }

        internal bool Exist()
        {
            switch (m_SnapshotState)
            {
                case State.InDbGet:
                case State.InDbAdd:
                case State.Add:
                    return true;
                case State.InDbRemove:
                case State.Remove:
                    return false;
                default:
                    return false;
            }
        }

        internal BsonDocument? Find()
        {
            switch (m_SnapshotState)
            {
                case State.InDbGet:
                case State.InDbAdd:
                case State.Add:
                    return m_SnapshotValue;
                case State.InDbRemove:
                case State.Remove:
                    return null;
                default:
                    return null;
            }
        }

        private class LogAddRemove<TKey, TValue> : ILog where TKey : notnull where TValue : class
        {
            private readonly TRecord<TKey, TValue> m_Record;
            private readonly TValue? m_SavedValue;
            private readonly TRecord<TKey, TValue>.State m_SavedState;
            
            public LogAddRemove(TRecord<TKey, TValue> record)
            {
                m_Record = record;
                m_SavedValue = record.Value;
                m_SavedState = record.Stat;
            }

            public void Commit()
            {
                m_Record.m_Table.OnRecordChanged(m_Record, false, m_SavedState);
            }

            public void Rollback()
            {
                m_Record.m_Value = m_SavedValue;
                m_Record.m_State = m_SavedState;
            }

            public override string ToString()
            {
                return $"state={m_SavedState} value={m_SavedValue}";
            }
        }
    }
}