using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public interface ICreateKindAttrHandler
    {
        void InitKind(TypeContext tc, AttributeSyntax attr);
        void AddField(string name, IType type);
    }
    
    public class DefaultCreateKindAttrHandler : ICreateKindAttrHandler
    {
        private TypeContext m_TypeContext = null!;
        private AttributeSyntax m_Attr = null!;

        public void InitKind(TypeContext tc, AttributeSyntax attr)
        {
            m_TypeContext = tc;
            m_Attr = attr;
            var classType = TypeBuilder.I.ParseType(tc.TypeSyntax);
            tc.ClassKind = new ClassKind(classType);
        }

        public void AddField(string name, IType type)
        {
            var field = new FieldKind(name, type);
            m_TypeContext.ClassKind!.AddField(field);
        }
    }
}