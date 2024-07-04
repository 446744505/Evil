using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public abstract class BaseTypeAttrHandler : IAttributeHandler
    {
        public void Parse(TypeContext tc, AttributeSyntax attr)
        {
            tc.Namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(tc.NameSpaceName));
            tc.Class = SyntaxFactory.ClassDeclaration(tc.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)) // 设置为public
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword)); // 设置为partial
            Parse0(tc, attr);
            ParseFinish(tc);
        }

        protected void ParseFinish(TypeContext tc)
        {
            tc.Namespace = tc.Namespace.AddMembers(tc.Class);
            tc.FileContext.AddNamespace(tc.Namespace);
        }

        protected abstract void Parse0(TypeContext tc, AttributeSyntax attr);
        public abstract string GetAttrName();
    }
}