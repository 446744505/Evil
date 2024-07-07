using System.Linq;
using Generator.Context;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
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
            var modifiers = SyntaxFactory.TokenList();
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword)); // 设置为public
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword)); // 设置为partial
            tc.NewNamespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(tc.OldNameSpaceName))
                .WithUsings(AnalysisUtil.SkipAttributes(tc.OldNameSpaceSyntax.Usings));
            tc.NewClassSyntax = SyntaxFactory.ClassDeclaration(tc.OldClassName)
                .AddModifiers(modifiers.ToArray());
        }

        public void FinishSyntax()
        {
            var tc = m_TypeContext;
            tc.NewNamespaceSyntax = tc.NewNamespaceSyntax!.AddMembers(tc.NewClassSyntax!);
            tc.FileContext.AddNamespaceSyntax(tc.NewNamespaceSyntax);
        }
    }
}