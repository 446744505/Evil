using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class TypeContext
    {
        #region 字段

        private readonly FileContext m_Fc;
        private readonly TypeDeclarationSyntax m_Type;
        private readonly string m_NameSpaceName;
        private readonly string m_ClassName;

        private NamespaceDeclarationSyntax m_Namespace;
        private ClassDeclarationSyntax m_Clazz;

        #endregion

        #region 属性

        public TypeDeclarationSyntax Type => m_Type;
        public string NameSpaceName => m_NameSpaceName;
        public string ClassName => m_ClassName;
        public FileContext FileContext => m_Fc;
        public NamespaceDeclarationSyntax Namespace
        {
            get => m_Namespace;
            set => m_Namespace = value;
        }

        public ClassDeclarationSyntax Class
        {
            get => m_Clazz;
            set => m_Clazz = value;
        }

        #endregion

        public TypeContext(FileContext fc, TypeDeclarationSyntax type)
        {
            m_Fc = fc;
            m_Type = type;
            m_NameSpaceName = AnalysisUtil.GetNameSpaceName(type);
            m_ClassName = AnalysisUtil.GetClassName(type);
        }
    }
}