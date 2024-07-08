
using Generator.Context;
using Generator.Visitor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Type
{
    public interface IType
    {
        public IType Parse(TypeSyntax typeSyntax);
        public IType Compile(CompileContext ctx);
        public void Accept(ITypeVisitor visitor);
    }
}