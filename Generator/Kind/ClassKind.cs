namespace Generator
{
    public class ClassKind
    {
        #region 字段

        private readonly ClassType m_Type;
        private readonly List<FieldKind> m_Fields = new();

        #endregion
        
        #region 属性
        
        public string Name => m_Type.ClassName;
        
        #endregion
        
        public ClassKind(ClassType type)
        {
            m_Type = type;
        }
        
        public void AddField(FieldKind field)
        {
            m_Fields.Add(field);
        }
    }
}