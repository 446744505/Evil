using Generator.Context;
using Generator.Kind;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public class XTableAttrHandler : BaseTypeAttrHandler, ICreateKindAttrHandler
    {
        private readonly ICreateKindAttrHandler m_CreateKindAttrHandler;
        public XTableAttrHandler(TypeContext typeContext, AttributeSyntax attr) : base(typeContext, attr)
        {
            // m_CreateKindAttrHandler = new DefaultCreateKindAttrHandler()
            // {
            //     NameSpaceSuffix = Namespaces.EdbNamespace
            // };
            // AnalysisUtil.HadAttrArgument(attr, AttributeFields.XTableNodes, out var nodes);
            // NeedParse = TypeContext.FileContext.GloableContext.IsNodeAt(nodes);
        }
        
        protected override void Parse0()
        {
            
        }

        public void InitKind(TypeContext tc, AttributeSyntax attr)
        {
            // m_CreateKindAttrHandler.InitKind(tc, attr);
        }

        public FieldKind NewField(NewFieldContext ctx)
        {
            return m_CreateKindAttrHandler.NewField(ctx);
        }
    }
}