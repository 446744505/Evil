using Generator.Type;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Context
{
    public class NewFieldContext
    {
        #region 字段

        private readonly string m_Name;
        private readonly IType m_Type;
        private string m_Comment;

        #endregion

        #region MyRegion

        public string Name => m_Name;
        public IType Type => m_Type;
        public string Comment => m_Comment;

        #endregion
        
        public NewFieldContext(string name, IType type)
        {
            m_Type = type;
            m_Name = name;
        }

        public static NewFieldContext Parse(FieldDeclarationSyntax syntax)
        {
            var type = TypeBuilder.I.ParseType(syntax.Declaration.Type);
            var name = AnalysisUtil.GetFieldName(syntax);
            return new NewFieldContext(name, type)
            {
                m_Comment = AnalysisUtil.GetFieldComment(syntax)
            };
        }
    }
}