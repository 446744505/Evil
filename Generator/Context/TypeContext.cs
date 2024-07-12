using Generator.Kind;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Context
{
    public class TypeContext
    {
        #region 属性

        public TypeDeclarationSyntax OldTypeSyntax { get; }
        public string OldNameSpaceName => OldNameSpaceSyntax.Name.ToString();
        public NamespaceDeclarationSyntax OldNameSpaceSyntax { get; }
        public string OldClassName { get; }
        public FileContext FileContext { get; }
        public NamespaceDeclarationSyntax? NewNamespaceSyntax { get; set; }
        public string NewNameSpaceName => NewNamespaceSyntax!.Name.ToString();

        public ClassDeclarationSyntax? NewClassSyntax { get; set; }

        public BaseIdentiferKind? ClassKind { get; set; }

        #endregion

        public TypeContext(FileContext fc, TypeDeclarationSyntax oldTypeSyntax)
        {
            FileContext = fc;
            OldTypeSyntax = oldTypeSyntax;
            OldNameSpaceSyntax = AnalysisUtil.GetNameSpaceSyntax(oldTypeSyntax);
            OldClassName = AnalysisUtil.GetClassName(oldTypeSyntax);
            fc.AddTypeContext(this);
        }
    }
}