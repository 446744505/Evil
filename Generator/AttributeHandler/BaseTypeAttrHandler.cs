
using Generator.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public abstract class BaseTypeAttrHandler : IAttributeHandler
    {
        public TypeContext TypeContext { get; }
        protected AttributeSyntax Attr { get; }
        protected bool NeedParse { get; set; } = true;

        protected BaseTypeAttrHandler(TypeContext typeContext, AttributeSyntax attr)
        {
            TypeContext = typeContext;
            Attr = attr;
        }

        public void Parse()
        {
            if (!NeedParse) return;
            
            var createKindAttrHandler = this as ICreateKindAttrHandler;
            createKindAttrHandler?.InitKind(TypeContext, Attr);
            var createSyntaxAttrHandler = this as ICreateSyntaxAttrHandler;
            createSyntaxAttrHandler?.InitSyntax(TypeContext, Attr);
            
            Parse0();
            
            createSyntaxAttrHandler?.FinishSyntax();
        }

        protected abstract void Parse0();
    }
}