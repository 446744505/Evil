using Generator.Context;
using Generator.Type;

namespace Generator.Kind
{
    public abstract class BaseIdentiferKind : BaseKind
    {
        #region 字段

        private readonly BaseIdentiferType m_Type;
        private readonly List<FieldKind> m_Fields = new();

        #endregion
        
        #region 属性
        
        public string Name => m_Type.Name;
        
        #endregion
        
        public BaseIdentiferKind(BaseIdentiferType type, IKind parent) : base(parent)
        {
            m_Type = type;
        }
        
        public void AddField(FieldKind field)
        {
            m_Fields.Add(field);
        }

        protected override void Compile0(CompileContext ctx)
        {
            m_Type.Compile(ctx);
            foreach (var field in m_Fields)
            {
                field.Compile(ctx);
            }
        }
        
        public new List<FieldKind> Children()
        {
            return m_Fields;
        }
        
        public abstract BaseIdentiferType CreateIdentiferType();
    }
}