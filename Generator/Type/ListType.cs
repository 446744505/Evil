
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class ListType : IType, IHaveValue
    {
        private IType m_Value = null!;
        
        public IType Value()
        {
            return m_Value;
        }

        public IType Parse(TypeSyntax typeSyntax)
        {
            var genericSyntax = (GenericNameSyntax) typeSyntax;
            m_Value = TypeBuilder.I.ParseType(genericSyntax.TypeArgumentList.Arguments[0]);
            return this;
        }

        public IType Compile(CompileContext ctx)
        {
            m_Value = m_Value.Compile(ctx);
            return this;
        }

        public void Accept<T>(ITypeVisitor<T> visitor) where T : ITypeVisitorContext
        {
            visitor.Visit(this);
        }
    }
}