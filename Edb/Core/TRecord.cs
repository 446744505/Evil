namespace Edb
{
    internal sealed class TRecord<TK, TV> : XBean
    {
        private const string RecordVarName = "m_Value";

        #region 字段

        private readonly TTable<TK, TV> m_Table;
        private readonly Lockey m_Lockey;
        private TV m_Value;
        private State m_State;
        private long m_LastAccessTime = DateTime.Now.Nanosecond;

        #endregion


        #region 属性

        internal long LastAccessTime => Interlocked.Read(ref m_LastAccessTime);
        internal TK Key => (TK)m_Lockey.Key;
        internal Lockey Lockey => m_Lockey;
        internal State Stat => m_State;

        #endregion
        

        internal TRecord(TTable<TK, TV> table, TV value, Lockey lockey, State state) : base(null, RecordVarName)
        {
            m_Table = table;
            if (value != null)
                Logs.Link(value, this, RecordVarName, State.InDbGet != state);
            m_Value = value;
            m_Lockey = lockey;
            m_State = state;
        }

        internal void Access()
        {
            var now = DateTime.Now.Nanosecond;
            Interlocked.Exchange(ref m_LastAccessTime, now);
        }

        internal override void Notify(LogNotify notify)
        {
            m_Table.OnRecordChanged(this, notify);
        }
        
        internal LogKey GetLogKey()
        {
            return new LogKey(this, RecordVarName);
        }

        public override string ToString()
        {
            return $"{m_Table.Name},{m_Lockey},{m_State}";
        }

        internal enum State
        {
            InDbGet,
            InDbAdd,
            InDbRemove,
            Add,
            Remove,
        }
    }
}