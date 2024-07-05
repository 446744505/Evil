
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public interface IType
    {
        public IType Parse(TypeSyntax typeSyntax);
        public void Compile();
    }
}