namespace Edb
{
    internal sealed class LogKey : IComparable<LogKey>
    {
        private readonly XBean m_XBean;
        private readonly string? m_VarName;
        
        internal XBean XBean => m_XBean;
        internal string? VarName => m_VarName;
        internal object Value { get; set;}

        internal LogKey(XBean xBean, string? varName)
        {
            m_XBean = xBean;
            m_VarName = varName;
        }

        public override int GetHashCode()
        {
            return m_XBean.ObjId.GetHashCode() ^ m_VarName.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj is LogKey other)
                return m_XBean.ObjId == other.m_XBean.ObjId && m_VarName == other.m_VarName;
            return false;
        }

        public int CompareTo(LogKey? other)
        {
            var x = m_XBean.ObjId - other!.m_XBean.ObjId;
            return (int)(x != 0 ? x : string.Compare(m_VarName, other.m_VarName, StringComparison.Ordinal));
        }

        public override string ToString()
        {
            return $"{m_XBean.GetType()}.{m_VarName}";
        }
    }
}