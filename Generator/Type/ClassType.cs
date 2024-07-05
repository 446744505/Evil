
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class ClassType : IType
    {
        private string m_ClassName;

        public string ClassName => m_ClassName;
        
        public IType Parse(TypeSyntax typeSyntax)
        {
            var identifierSyntax = (IdentifierNameSyntax) typeSyntax;
            // 获取类型名称
            m_ClassName = identifierSyntax.Identifier.Text;
            return this;
        }
        
        public ClassType Parse(TypeDeclarationSyntax typeSyntax)
        {
            m_ClassName = typeSyntax.Identifier.Text;
            return this;
        }

        public void Compile()
        {
            throw new NotImplementedException();
        }
    }
}