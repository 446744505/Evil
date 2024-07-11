using Generator.Kind;
using Generator.Proto;
using Generator.Type;

namespace Generator.Visitor
{
    /// <summary>
    /// 计算生成proto文件时每个字段的一行
    /// </summary>
    public class ProtoFieldTypeVisitor : ITypeVisitor
    {
        private readonly ProtoFieldKind m_Field;
        private readonly ProtoContext m_Pc;
        public string FieldName => m_Field.Name;
        public int FieldIndex => m_Field.Index;
        
        public string Result { get; set; }
        
        public ProtoFieldTypeVisitor(FieldKind field, ProtoContext pc)
        {
            m_Pc = pc;
            m_Field = (ProtoFieldKind) field;
        }

        private void Visit0(IType type)
        {
            var typeNameVisitor = new ProtoTypeNameTypeVisitor(m_Pc);
            type.Accept(typeNameVisitor);
            if (string.IsNullOrEmpty(m_Field.Comment))
                Result = $"{typeNameVisitor.Result} {FieldName} = {FieldIndex};";
            else
                Result = $"{typeNameVisitor.Result} {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }
        
        public void Visit(StructType type)
        {
            Visit0(type);
        }

        public void Visit(ClassType type)
        {
            Visit0(type);
        }

        public void Visit(IntType type)
        {
            Visit0(type);
        }

        public void Visit(LongType type)
        {
            Visit0(type);
        }

        public void Visit(BoolType type)
        {
            Visit0(type);
        }

        public void Visit(StringType type)
        {
            Visit0(type);
        }

        public void Visit(FloatType type)
        {
            Visit0(type);
        }

        public void Visit(DoubleType type)
        {
            Visit0(type);
        }

        public void Visit(ListType type)
        {
            Visit0(type);
        }

        public void Visit(MapType type)
        {
            Visit0(type);
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