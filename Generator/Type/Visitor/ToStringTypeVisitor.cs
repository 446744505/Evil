using Generator.Kind;
using Generator.Type;

namespace Generator.Visitor
{
    public class ToStringTypeVisitor : ITypeVisitor
    {
        private readonly FieldKind m_FieldKind;
        
        public ToStringTypeVisitor(FieldKind fieldKind)
        {
            m_FieldKind = fieldKind;
        }
        
        private string FieldName => m_FieldKind.Name;
        public string Result { get; private set; }
        
        private void SimpleVisit()
        {
            Result = $"{FieldName}";
        }
        
        public void Visit(StructType type)
        {
            SimpleVisit();
        }

        public void Visit(ClassType type)
        {
            SimpleVisit();
        }

        public void Visit(IntType type)
        {
            SimpleVisit();
        }

        public void Visit(LongType type)
        {
            SimpleVisit();
        }

        public void Visit(BoolType type)
        {
            SimpleVisit();
        }

        public void Visit(StringType type)
        {
            SimpleVisit();
        }

        public void Visit(FloatType type)
        {
            SimpleVisit();
        }

        public void Visit(DoubleType type)
        {
            SimpleVisit();
        }
        
        private void CollectionVisit()
        {
            Result = $"Evil.Util.Strings.ToCustomString({FieldName})";
        }

        public void Visit(ListType type)
        {
            CollectionVisit();
        }

        public void Visit(MapType type)
        {
            CollectionVisit();
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