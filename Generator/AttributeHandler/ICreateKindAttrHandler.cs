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
        void InitKind(TypeContext tc, AttributeSyntax attr);
        FieldKind NewField(NewFieldContext ctx);
    }
    
    public class DefaultCreateKindAttrHandler : ICreateKindAttrHandler
    {
        private TypeContext m_TypeContext = null!;
        private AttributeSyntax m_Attr = null!;

        public ICreateFieldFactory<FieldKind> CreateFieldFactory { get; set; } = new DefaultCreateFieldFactory();
        /// <summary>
        /// 附加的命名空间后缀
        /// </summary>
        public string NameSpaceSuffix { get; set; } = "";

        public void InitKind(TypeContext tc, AttributeSyntax attr)
        {
            m_TypeContext = tc;
            m_Attr = attr;
            var identiferType = TypeBuilder.I.ParseType(tc.OldTypeSyntax);
            var nameSpace = tc.OldNameSpaceName;
            if (!string.IsNullOrEmpty(NameSpaceSuffix))
            {
                nameSpace += "." + NameSpaceSuffix;
            }
            var namespaceKind = tc.FileContext.GetOrCreateNamespaceKind(nameSpace);
            tc.ClassKind = identiferType.CreateKind(namespaceKind);
            tc.ClassKind.Comment = AnalysisUtil.GetComment(tc.OldTypeSyntax);
        }

        public FieldKind NewField(NewFieldContext ctx)
        {
            var field = CreateFieldFactory.CreateField(ctx, m_TypeContext.ClassKind!);
            m_TypeContext.ClassKind!.AddField(field);
            return field;
        }
    }
}