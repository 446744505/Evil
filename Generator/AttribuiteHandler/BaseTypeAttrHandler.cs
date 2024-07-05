
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public abstract class BaseTypeAttrHandler : IAttributeHandler
    {
        public void Parse(TypeContext tc, AttributeSyntax attr)
        {
            var createKindAttrHandler = this as ICreateKindAttrHandler;
            createKindAttrHandler?.InitKind(tc, attr);
            var createSyntaxAttrHandler = this as ICreateSyntaxAttrHandler;
            createSyntaxAttrHandler?.InitSyntax(tc, attr);
            
            Parse0(tc, attr);
            
            createSyntaxAttrHandler?.FinishSyntax();
        }

        protected abstract void Parse0(TypeContext tc, AttributeSyntax attr);
        public abstract string GetAttrName();
    }
}