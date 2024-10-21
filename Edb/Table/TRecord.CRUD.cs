using MongoDB.Bson;

namespace Edb
{
    internal partial class TRecord<TKey, TValue> : XBean
        where TKey : notnull where TValue : class
    {
        private void Remove0(TransactionCtx ctx)
        {
            Logs.Link(m_Value, null, null!, ctx);
            ctx.Current!.CurrentSavepoint.AddIfAbsent(CreateLogKey(), new LogAddRemove<TKey, TValue>(this));
            m_Value = null;
        }

        internal bool Remove(TransactionCtx ctx)
        {
            switch (m_State)
            {
                case State.InDbGet:
                    Remove0(ctx);
                    m_State = State.InDbRemove;
                    return true;
                case State.InDbAdd:
                    Remove0(ctx);
                    m_State = State.InDbRemove;
                    return true;
                case State.Add:
                    Remove0(ctx);
                    m_State = State.Remove;
                    return true;
                default:
                    return false;
            }
        }

        private void Add0(TValue value, TransactionCtx ctx)
        {
            Logs.Link(value, this, RecordVarName, ctx);
            ctx.Current!.CurrentSavepoint.AddIfAbsent(CreateLogKey(), new LogAddRemove<TKey, TValue>(this));
            m_Value = value;
        }

        internal bool Add(TValue value, TransactionCtx ctx)
        {
            switch (m_State)
            {
                case State.InDbRemove:
                    Add0(value, ctx);
                    m_State = State.InDbAdd;
                    return true;
                case State.Remove:
                    Add0(value, ctx);
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

        private class LogAddRemove<TK, TV> : ILog where TK : notnull where TV : class
        {
            private readonly TRecord<TK, TV> m_Record;
            private readonly TV? m_SavedValue;
            private readonly TRecord<TK, TV>.State m_SavedState;
            
            public LogAddRemove(TRecord<TK, TV> record)
            {
                m_Record = record;
                m_SavedValue = record.Value;
                m_SavedState = record.Stat;
            }

            public void Commit(TransactionCtx ctx)
            {
                m_Record.m_Table.OnRecordChanged(m_Record, false, m_SavedState, ctx);
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