using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public interface IAttributeHandler
    {
        void Parse(TypeContext tc, AttributeSyntax attr);
    }
}