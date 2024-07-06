using Generator.Context;
using Generator.Factory;
using Generator.Kind;
using Generator.Type;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public interface ICreateKindAttrHandler
    {
        void InitKind(TypeContext tc, AttributeSyntax attr);
        FieldKind NewField(string name, IType type);
    }
    
    public class DefaultCreateKindAttrHandler : ICreateKindAttrHandler
    {
        private TypeContext m_TypeContext = null!;
        private AttributeSyntax m_Attr = null!;
        private ICreateFieldFactory<FieldKind> m_CreateFieldFactory = new DefaultCreateFieldFactory();
        
        public ICreateFieldFactory<FieldKind> CreateFieldFactory
        {
            get => m_CreateFieldFactory;
            set => m_CreateFieldFactory = value;
        }

        public void InitKind(TypeContext tc, AttributeSyntax attr)
        {
            m_TypeContext = tc;
            m_Attr = attr;
            var identiferType = TypeBuilder.I.ParseType(tc.TypeSyntax);
            var namespaceKind = tc.FileContext.GetOrCreateNamespaceKind(tc.NameSpaceName);
            tc.ClassKind = identiferType.CreateKind(namespaceKind);
        }

        public FieldKind NewField(string name, IType type)
        {
            var field = m_CreateFieldFactory.CreateField(name, type, m_TypeContext.ClassKind!);
            m_TypeContext.ClassKind!.AddField(field);
            return field;
        }
    }
}