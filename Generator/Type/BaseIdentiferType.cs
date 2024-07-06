using Generator.Context;
using Generator.Kind;
using Generator.Visitor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Type
{
    public abstract class BaseIdentiferType  : IType
    {
        protected string m_Name;

        public string Name => m_Name;
        
        public virtual IType Parse(TypeSyntax typeSyntax)
        {
            var identifierSyntax = (IdentifierNameSyntax) typeSyntax;
            m_Name = identifierSyntax.Identifier.Text;
            return this;
        }
        
        public BaseIdentiferType Parse(TypeDeclarationSyntax typeSyntax)
        {
            m_Name = typeSyntax.Identifier.Text;
            return this;
        }

        public abstract IType Compile(CompileContext ctx);
        public abstract BaseIdentiferKind CreateKind(IKind parent);
        public abstract void Accept<T>(ITypeVisitor<T> visitor) where T : ITypeVisitorContext;
    }
}