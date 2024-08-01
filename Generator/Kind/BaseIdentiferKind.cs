using System.Collections.Generic;
using Generator.Context;
using Generator.Type;

namespace Generator.Kind
{
    public abstract partial class BaseIdentiferKind : BaseKind
    {
        #region 字段
        
        private readonly List<FieldKind> m_Fields = new();

        #endregion
        
        #region 属性
        
        public string Name => Type.Name;
        public string Comment { get; set; }
        public BaseIdentiferType Type { get; }

        #endregion
        
        public BaseIdentiferKind(BaseIdentiferType type, IKind parent) : base(parent)
        {
            Type = type;
        }
        
        public void AddField(FieldKind field)
        {
            m_Fields.Add(field);
        }
        
        public string NamespaceName()
        {
            var parent = Parent();
            if (parent is NamespaceKind namespaceKind)
            {
                return namespaceKind.Name;
            }
            return "";
        }
        
        public string FullName()
        {
            var nameSpace = NamespaceName();
            if (string.IsNullOrEmpty(nameSpace))
            {
                return Name;
            }
            return $"{nameSpace}.{Name}";
        }

        protected override void Compile0(CompileContext ctx)
        {
            Type.Compile(ctx);
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