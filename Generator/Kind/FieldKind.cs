namespace Generator
{
    public class FieldKind
    {
        #region 字段

        private readonly string m_Name;
        private readonly IType m_Type;

        #endregion
        
        #region 属性
        
        public string Name => m_Name;
        
        #endregion
        
        public FieldKind(string name, IType type)
        {
            m_Name = name;
            m_Type = type;
        }
    }
}