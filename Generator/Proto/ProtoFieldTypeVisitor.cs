using Generator.Kind;
using Generator.Proto;
using Generator.Type;

namespace Generator.Visitor
{
    public class ProtoTypeVisitorContext : ITypeVisitorContext
    {
        public string Line { get; set; }
    }
    public class ProtoFieldTypeVisitor : BaseTypeVisitor<ProtoTypeVisitorContext>
    {
        private readonly ProtoFieldKind m_Field;
        private readonly ProtoContext m_Context;
        public string FieldName => m_Field.Name;
        public int FieldIndex => m_Field.Index;
        public ProtoFieldTypeVisitor(FieldKind field, ProtoContext ctx) : base(new ProtoTypeVisitorContext())
        {
            m_Context = ctx;
            m_Field = (ProtoFieldKind) field;
        }

        private void Visit0(IType type)
        {
            var typeNameVisitor = new ProtoTypeNameTypeVisitor(m_Context);
            type.Accept(typeNameVisitor);
            if (string.IsNullOrEmpty(m_Field.Comment))
                Context.Line = $"{typeNameVisitor.Context.Name} {FieldName} = {FieldIndex};";
            else
                Context.Line = $"{typeNameVisitor.Context.Name} {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }
        
        public override void Visit(StructType type)
        {
            Visit0(type);
        }

        public override void Visit(ClassType type)
        {
            Visit0(type);
        }

        public override void Visit(IntType type)
        {
            Visit0(type);
        }

        public override void Visit(LongType type)
        {
            Visit0(type);
        }

        public override void Visit(BoolType type)
        {
            Visit0(type);
        }

        public override void Visit(StringType type)
        {
            Visit0(type);
        }

        public override void Visit(FloatType type)
        {
            Visit0(type);
        }

        public override void Visit(DoubleType type)
        {
            Visit0(type);
        }

        public override void Visit(ListType type)
        {
            Visit0(type);
        }

        public override void Visit(MapType type)
        {
            Visit0(type);
        }
        
    }
}