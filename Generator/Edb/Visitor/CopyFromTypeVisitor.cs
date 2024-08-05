using Generator.Kind;
using Generator.Type;
using Generator.Util;

namespace Generator.Visitor
{
    public class CopyFromTypeVisitor : ITypeVisitor
    {
        private readonly XBeanFieldKind m_FieldKind;
        private readonly Writer m_Writer;

        private string FieldName => m_FieldKind.Name;
        
        public CopyFromTypeVisitor(XBeanFieldKind fieldKind, Writer writer)
        {
            m_FieldKind = fieldKind;
            m_Writer = writer;
        }
        
        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            m_Writer.WriteLine($"{FieldName}.CopyFrom(_o_.{FieldName});");
        }

        private void BaseVisit()
        {
            m_Writer.WriteLine($"Edb.Logs.LogObject(this, \"{FieldName}\");");
            m_Writer.WriteLine($"{FieldName} = _o_.{FieldName};");
        }
        
        public void Visit(IntType type)
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

        public void Visit(ListType type)
        {
            var valueCopyVisitor = new CopyFieldTypeVisitor(m_FieldKind, "_v_");
            type.Value().Accept(valueCopyVisitor);
            var valueFullNameTypeVisitor = new FullNameTypeVisitor();
            type.Value().Accept(valueFullNameTypeVisitor);
            m_Writer.WriteLine($"var _{FieldName}_ = Edb.Logs.LogList<{valueFullNameTypeVisitor.Result}>(this, \"{FieldName}\", DoNothing);");
            m_Writer.WriteLine($"_{FieldName}_.Clear();");
            m_Writer.WriteLine($@"foreach (var _v_ in _o_.{FieldName})
            {{
                _{FieldName}_.Add({valueCopyVisitor.Result});
            }}");
        }

        public void Visit(MapType type)
        {
            var valueCopyVisitor = new CopyFieldTypeVisitor(m_FieldKind, "_pair_.Value");
            type.Value().Accept(valueCopyVisitor);
            var keyFullNameTypeVisitor = new FullNameTypeVisitor();
            type.Key().Accept(keyFullNameTypeVisitor);
            var valueFullNameTypeVisitor = new FullNameTypeVisitor();
            type.Value().Accept(valueFullNameTypeVisitor);
            m_Writer.WriteLine($"var _{FieldName}_ = Edb.Logs.LogMap<{keyFullNameTypeVisitor.Result}, {valueFullNameTypeVisitor.Result}>(this, \"{FieldName}\", DoNothing);");
            m_Writer.WriteLine($"_{FieldName}_.Clear();");
            m_Writer.WriteLine($@"foreach (var _pair_ in _o_.{FieldName})
            {{
                {FieldName}.Add(_pair_.Key, {valueCopyVisitor.Result});
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