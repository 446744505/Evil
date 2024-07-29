using Generator.Context;
using Generator.Factory;
using Generator.Kind;
using Generator.Type;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public interface ICreateKindAttrHandler
    {
        public ICreateNamespaceFactory CreateNamespaceFactory { get; }
        void InitKind(TypeContext tc, AttributeSyntax attr);
        FieldKind NewField(NewFieldContext ctx);
    }
    
    public class DefaultCreateKindAttrHandler : ICreateKindAttrHandler
    {
        private TypeContext m_TypeContext = null!;
        private AttributeSyntax m_Attr = null!;

        public ICreateNamespaceFactory CreateNamespaceFactory { get; set; } = new DefaultCreateNamespaceFactory();
        public ICreateIdentiferFactory CreateIdentiferFactory { get; set; } = new DefaultCreateIdentiferFactory();
        public ICreateFieldFactory<FieldKind> CreateFieldFactory { get; set; } = new DefaultCreateFieldFactory();
        /// <summary>
        /// 附加的命名空间后缀
        /// </summary>
        public string NamespaceSuffix { get; set; } = "";
        public string ForceNamespace { get; set; } = "";

        public void InitKind(TypeContext tc, AttributeSyntax attr)
        {
            m_TypeContext = tc;
            m_Attr = attr;
            var identiferType = TypeBuilder.I.ParseType(tc.OldTypeSyntax);
            var nameSpace = ForceNamespace;
            if (string.IsNullOrEmpty(nameSpace))
            {
                nameSpace = tc.OldNameSpaceName;
                if (!string.IsNullOrEmpty(NamespaceSuffix))
                {
                    nameSpace += "." + NamespaceSuffix;
                }
            }
            var namespaceKind = tc.FileContext.GetOrCreateNamespaceKind(nameSpace, CreateNamespaceFactory);
            tc.IdentiferKind = CreateIdentiferFactory.CreateIdentifer(identiferType, namespaceKind);
            tc.IdentiferKind.Comment = AnalysisUtil.GetComment(tc.OldTypeSyntax);
        }

        public FieldKind NewField(NewFieldContext ctx)
        {
            var field = CreateFieldFactory.CreateField(ctx, m_TypeContext.IdentiferKind!);
            m_TypeContext.IdentiferKind!.AddField(field);
            return field;
        }
    }
}