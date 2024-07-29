using Generator.Kind;
using Generator.Type;

namespace Generator.Visitor
{
    public class CopyFieldTypeVisitor : ITypeVisitor
    {
        private readonly XBeanFieldKind m_FieldKind;
        private readonly string m_CopyFieldName;

        private string FieldName => m_FieldKind.Name;
        public string Result;
        
        public CopyFieldTypeVisitor(XBeanFieldKind fieldKind, string copyFieldName)
        {
            m_FieldKind = fieldKind;
            m_CopyFieldName = copyFieldName;
        }

        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        private void BaseVisit(IType type)
        {
            var fullNameVisitor = new FullNameTypeVisitor();
            type.Accept(fullNameVisitor);
            Result = fullNameVisitor.Result;
        }

        public void Visit(ClassType type)
        {
            Result = $"new({m_CopyFieldName})";
        }

        public void Visit(IntType type)
        {
            BaseVisit(type);
        }

        public void Visit(LongType type)
        {
            BaseVisit(type);
        }

        public void Visit(BoolType type)
        {
            BaseVisit(type);
        }

        public void Visit(StringType type)
        {
            BaseVisit(type);
        }

        public void Visit(FloatType type)
        {
            BaseVisit(type);
        }

        public void Visit(DoubleType type)
        {
            BaseVisit(type);
        }

        public void Visit(ListType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MapType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(TaskType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(WaitCompileIdentiferType type)
        {
            throw new System.NotImplementedException();
        }
    }
}