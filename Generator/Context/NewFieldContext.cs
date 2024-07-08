using Generator.Type;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Context
{
    public class NewFieldContext
    {
        #region MyRegion

        public string Name { get; }
        public IType Type { get; }
        public string? Comment { get; set; }

        #endregion
        
        public NewFieldContext(string name, IType type)
        {
            Type = type;
            Name = name;
        }

        public static NewFieldContext Parse(FieldDeclarationSyntax syntax)
        {
            var type = TypeBuilder.I.ParseType(syntax.Declaration.Type);
            var name = AnalysisUtil.GetFieldName(syntax);
            return new NewFieldContext(name, type)
            {
                Comment = AnalysisUtil.GetFieldComment(syntax)
            };
        }
    }
}