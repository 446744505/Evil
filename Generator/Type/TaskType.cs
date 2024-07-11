
using Generator.Context;
using Generator.Visitor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Type
{
    public class TaskType : IType, IHaveValue
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

        public void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}