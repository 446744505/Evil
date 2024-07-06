using Generator.Kind;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Context
{
    public class TypeContext
    {
        #region 字段

        private readonly FileContext m_Fc;
        private readonly TypeDeclarationSyntax m_TypeSyntax;
        private readonly string m_NameSpaceName;
        private readonly string m_ClassName;

        private NamespaceDeclarationSyntax? m_NamespaceSyntax;
        private ClassDeclarationSyntax? m_ClazzSyntax;
        private BaseIdentiferKind? m_ClassKind;

        #endregion

        #region 属性

        public TypeDeclarationSyntax TypeSyntax => m_TypeSyntax;
        public string NameSpaceName => m_NameSpaceName;
        public string ClassName => m_ClassName;
        public FileContext FileContext => m_Fc;
        public NamespaceDeclarationSyntax? NamespaceSyntax
        {
            get => m_NamespaceSyntax;
            set => m_NamespaceSyntax = value;
        }

        public ClassDeclarationSyntax? ClassSyntax
        {
            get => m_ClazzSyntax;
            set => m_ClazzSyntax = value;
        }
        
        public BaseIdentiferKind? ClassKind
        {
            get => m_ClassKind;
            set => m_ClassKind = value;
        }

        #endregion

        public TypeContext(FileContext fc, TypeDeclarationSyntax typeSyntax)
        {
            m_Fc = fc;
            m_TypeSyntax = typeSyntax;
            m_NameSpaceName = AnalysisUtil.GetNameSpaceName(typeSyntax);
            m_ClassName = AnalysisUtil.GetClassName(typeSyntax);
            fc.AddTypeContext(this);
        }
    }
}