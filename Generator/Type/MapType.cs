
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class MapType : IType, IHaveKey, IHaveValue
    {
        private IType m_Key = null!;
        private IType m_Value = null!;
        
        public IType Key()
        {
            return m_Key;    
        }

        public IType Value()
        {
            return m_Value;
        }

        public IType Parse(TypeSyntax typeSyntax)
        {
            var genericSyntax = (GenericNameSyntax) typeSyntax;
            m_Key = TypeBuilder.I.ParseType(genericSyntax.TypeArgumentList.Arguments[0]);
            m_Value = TypeBuilder.I.ParseType(genericSyntax.TypeArgumentList.Arguments[1]);
            return this;
        }

        public IType Compile(CompileContext ctx)
        {
            m_Key = m_Key.Compile(ctx);
            m_Value = m_Value.Compile(ctx);
            return this;
        }

        public void Accept<T>(ITypeVisitor<T> visitor) where T : ITypeVisitorContext
        {
            visitor.Visit(this);
        }
    }
}