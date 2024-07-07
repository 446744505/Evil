using Generator.Kind;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Context
{
    public class TypeContext
    {
        #region 字段

        private readonly FileContext m_Fc;
        private readonly TypeDeclarationSyntax m_OldTypeSyntax;
        private readonly NamespaceDeclarationSyntax m_OldNameSpaceSyntax;
        private readonly string m_OldClassName;

        private NamespaceDeclarationSyntax? m_NewNamespaceSyntax;
        private ClassDeclarationSyntax? m_NewClazzSyntax;
        private BaseIdentiferKind? m_ClassKind;

        #endregion

        #region 属性

        public TypeDeclarationSyntax OldTypeSyntax => m_OldTypeSyntax;
        public string OldNameSpaceName => m_OldNameSpaceSyntax.Name.ToString();
        public NamespaceDeclarationSyntax OldNameSpaceSyntax => m_OldNameSpaceSyntax;
        public string OldClassName => m_OldClassName;
        public FileContext FileContext => m_Fc;
        public NamespaceDeclarationSyntax? NewNamespaceSyntax
        {
            get => m_NewNamespaceSyntax;
            set => m_NewNamespaceSyntax = value;
        }

        public ClassDeclarationSyntax? NewClassSyntax
        {
            get => m_NewClazzSyntax;
            set => m_NewClazzSyntax = value;
        }
        
        public BaseIdentiferKind? ClassKind
        {
            get => m_ClassKind;
            set => m_ClassKind = value;
        }

        #endregion

        public TypeContext(FileContext fc, TypeDeclarationSyntax oldTypeSyntax)
        {
            m_Fc = fc;
            m_OldTypeSyntax = oldTypeSyntax;
            m_OldNameSpaceSyntax = AnalysisUtil.GetNameSpaceName(oldTypeSyntax);
            m_OldClassName = AnalysisUtil.GetClassName(oldTypeSyntax);
            fc.AddTypeContext(this);
        }
    }
}