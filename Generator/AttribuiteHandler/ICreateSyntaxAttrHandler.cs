using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public interface ICreateSyntaxAttrHandler
    {
        public void InitSyntax(TypeContext tc, AttributeSyntax attr);
        public void FinishSyntax();
    }
    
    public class DefaultCreateSyntaxAttrHandler : ICreateSyntaxAttrHandler
    {
        private TypeContext m_TypeContext = null!;
        private AttributeSyntax m_Attr = null!;

        public void InitSyntax(TypeContext tc, AttributeSyntax attr)
        {
            m_TypeContext = tc;
            m_Attr = attr;
            tc.NamespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(tc.NameSpaceName));
            tc.ClassSyntax = SyntaxFactory.ClassDeclaration(tc.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)) // 设置为public
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword)); // 设置为partial
        }

        public void FinishSyntax()
        {
            var tc = m_TypeContext;
            tc.NamespaceSyntax = tc.NamespaceSyntax!.AddMembers(tc.ClassSyntax!);
            tc.FileContext.AddNamespace(tc.NamespaceSyntax);
        }
    }
}