using System.Linq;
using Generator.Context;
using Generator.Edb;
using Generator.Factory;
using Generator.Kind;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public class XBeanAttrHandler : BaseTypeAttrHandler, ICreateKindAttrHandler
    {
        private readonly ICreateKindAttrHandler m_CreateKindAttrHandler;
        
        public XBeanAttrHandler(TypeContext typeContext, AttributeSyntax attr) : base(typeContext, attr)
        {
            m_CreateKindAttrHandler = new DefaultCreateKindAttrHandler()
            {
                ForceNamespace = Namespaces.XBeanNamespace,
                CreateNamespaceFactory = new XBeanCreateNamespaceFactory(),
                CreateIdentiferFactory = new XBeanCreateIdentiferFactory(),
                CreateFieldFactory = new XBeanCreateFieldFactory()
            };
            AnalysisUtil.HadAttrArgument(attr, AttributeFields.XBeanNodes, out var nodes);
            NeedParse = TypeContext.FileContext.GloableContext.IsNodeAt(nodes);
        }
        
        protected override void Parse0()
        {
            var fields = TypeContext.OldTypeSyntax.DescendantNodes().OfType<FieldDeclarationSyntax>();
            foreach (var f in fields)
            {
                var ctx = NewFieldContext.Parse(f);
                NewField(ctx);
            }
        }

        public ICreateNamespaceFactory CreateNamespaceFactory => m_CreateKindAttrHandler.CreateNamespaceFactory;
        public void InitKind(TypeContext tc, AttributeSyntax attr)
        {
            m_CreateKindAttrHandler.InitKind(tc, attr);
        }

        public FieldKind NewField(NewFieldContext ctx)
        {
            return m_CreateKindAttrHandler.NewField(ctx);
        }
    }
}