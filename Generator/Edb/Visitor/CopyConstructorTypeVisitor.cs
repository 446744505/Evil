using Generator.Kind;
using Generator.Type;
using Generator.Util;

namespace Generator.Visitor
{
    public class CopyConstructorTypeVisitor : ITypeVisitor
    {
        private readonly XBeanFieldKind m_FieldKind;
        private readonly Writer m_Writer;
        
        private string FieldName => m_FieldKind.Name;

        public CopyConstructorTypeVisitor(XBeanFieldKind fieldKind, Writer writer)
        {
            m_FieldKind = fieldKind;
            m_Writer = writer;
        }
        
        private void BaseVisit()
        {
            m_Writer.WriteLine($"{FieldName} = _o_.{FieldName};");
        }

        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            var defaultValueVisitor = new EdbDefaultValueTypeVisitor(m_FieldKind, $"_o_.{FieldName}");
            type.Accept(defaultValueVisitor);
            m_Writer.WriteLine($"{FieldName} = {defaultValueVisitor.Result};");
        }

        public void Visit(ByteType type)
        {
            BaseVisit();
        }

        public void Visit(UShortType type)
        {
            BaseVisit();
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
            var defaultValueVisitor = new EdbDefaultValueTypeVisitor(m_FieldKind);
            type.Accept(defaultValueVisitor);
            m_Writer.WriteLine($"{FieldName} = {defaultValueVisitor.Result};");
            var valueDefaultValueVisitor = new EdbDefaultValueTypeVisitor(m_FieldKind, "_v_");
            type.Value().Accept(valueDefaultValueVisitor);
            m_Writer.WriteLine($@"foreach (var _v_ in _o_.{FieldName})
            {{
                {FieldName}.Add({valueDefaultValueVisitor.Result});
            }}");
        }

        public void Visit(MapType type)
        {
            var defaultValueVisitor = new EdbDefaultValueTypeVisitor(m_FieldKind);
            type.Accept(defaultValueVisitor);
            m_Writer.WriteLine($"{FieldName} = {defaultValueVisitor.Result};");
            var valueDefaultValueVisitor = new EdbDefaultValueTypeVisitor(m_FieldKind, "_pair_.Value");
            type.Value().Accept(valueDefaultValueVisitor);
            m_Writer.WriteLine($@"foreach (var _pair_ in _o_.{FieldName})
            {{
                {FieldName}.Add(_pair_.Key, {valueDefaultValueVisitor.Result});
            }}");
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