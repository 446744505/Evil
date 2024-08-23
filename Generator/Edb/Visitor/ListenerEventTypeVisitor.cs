using Generator.Type;
using Generator.Util;

namespace Generator.Visitor
{
    public class ListenerEventTypeVisitor : ITypeVisitor
    {
        private readonly Writer m_Writer;
        private readonly string m_ValFullName;
        private readonly string m_PropertiesName;

        public ListenerEventTypeVisitor(Writer writer, string valFullName, string propertiesName)
        {
            m_Writer = writer;
            m_ValFullName = valFullName;
            m_PropertiesName = propertiesName;
        }

        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ByteType type)
        {
            BaseVisit();
        }

        public void Visit(UShortType type)
        {
            BaseVisit();
        }

        private void BaseVisit()
        {
            m_Writer.Write($"public {m_ValFullName} {m_PropertiesName} {{ get; set; }}");
        }

        public void Visit(IntType type)
        {
            BaseVisit();
        }

        public void Visit(UIntType type)
        {
            BaseVisit();
        }

        public void Visit(LongType type)
        {
            BaseVisit();
        }

        public void Visit(BoolType type)
        {
            BaseVisit();
        }

        public void Visit(StringType type)
        {
            BaseVisit();
        }

        public void Visit(FloatType type)
        {
            BaseVisit();
        }

        public void Visit(DoubleType type)
        {
            BaseVisit();
        }

        public void Visit(ArrayType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ListType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MapType type)
        {
            m_Writer.WriteLine($"public bool IsAdd {{ get; set; }} // true for add, false for remove");
            var keyFullNameVisitor = new FullNameTypeVisitor();
            type.Key().Accept(keyFullNameVisitor);
            m_Writer.Write(2,$"public {keyFullNameVisitor.Result} MKey {{ get; set; }} // add or removed key");
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