namespace Generator
{
    public class FieldKind : BaseKind
    {
        #region 字段

        private readonly string m_Name;
        private readonly IType m_Type;

        #endregion
        
        #region 属性
        
        public string Name => m_Name;
        
        #endregion
        
        public FieldKind(string name, IType type, IKind parent) : base(parent)
        {
            m_Name = name;
            m_Type = type;
        }
    }
}