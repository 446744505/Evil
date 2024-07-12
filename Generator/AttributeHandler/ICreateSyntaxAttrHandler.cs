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
        protected TypeContext m_TypeContext = null!;
        protected AttributeSyntax m_Attr = null!;

        /// <summary>
        /// 是不是要创建文件
        /// </summary>
        public bool IsCreateFile { get; set; } = true;

        public void InitSyntax(TypeContext tc, AttributeSyntax attr)
        {
            m_TypeContext = tc;
            m_Attr = attr;
            
            NewNamespaceSyntax();
            NewClassSyntax();
        }
        
        protected virtual void NewNamespaceSyntax()
        {
            var tc = m_TypeContext;
            tc.NewNamespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(tc.OldNameSpaceName))
                    .WithUsings(AnalysisUtil.SkipAttributes(tc.OldNameSpaceSyntax.Usings));
        }
        
        protected virtual void NewClassSyntax()
        {
            var tc = m_TypeContext;
            var modifiers = SyntaxFactory.TokenList();
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword)); // 设置为public
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword)); // 设置为partial
            tc.NewClassSyntax = SyntaxFactory.ClassDeclaration(tc.OldClassName)
                .AddModifiers(modifiers.ToArray());
        }

        public virtual void FinishSyntax()
        {
            if (!IsCreateFile) return;
            
            var tc = m_TypeContext;
            tc.NewNamespaceSyntax = tc.NewNamespaceSyntax!.AddMembers(tc.NewClassSyntax!);
            tc.FileContext.AddNamespaceSyntax(tc.NewNamespaceSyntax);
        }
    }
}