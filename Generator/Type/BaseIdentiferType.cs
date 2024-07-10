using Generator.Context;
using Generator.Kind;
using Generator.Visitor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Type
{
    public abstract class BaseIdentiferType  : IType
    {
        public string Name { get; set; }
        
        public virtual IType Parse(TypeSyntax typeSyntax)
        {
            var identifierSyntax = (IdentifierNameSyntax) typeSyntax;
            Name = identifierSyntax.Identifier.Text;
            return this;
        }
        
        public BaseIdentiferType Parse(TypeDeclarationSyntax typeSyntax)
        {
            Name = typeSyntax.Identifier.Text;
            return this;
        }
        
        public BaseIdentiferType Parse(MethodDeclarationSyntax methodSyntax)
        {
            Name = methodSyntax.Identifier.Text;
            return this;
        }

        public abstract IType Compile(CompileContext ctx);
        public abstract BaseIdentiferKind CreateKind(IKind parent);
        public abstract void Accept(ITypeVisitor visitor);
    }
}