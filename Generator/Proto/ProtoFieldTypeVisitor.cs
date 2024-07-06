using Generator.Kind;
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
        public string FieldName => m_Field.Name;
        public int FieldIndex => m_Field.Index;
        public ProtoFieldTypeVisitor(FieldKind field) : base(new ProtoTypeVisitorContext())
        {
            m_Field = (ProtoFieldKind) field;
        }
        
        public override void Visit(StructType type)
        {
            Context.Line = $"{type.Name} {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(ClassType type)
        {
            Context.Line = $"{type.Name} {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(IntType type)
        {
            Context.Line = $"int32 {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(LongType type)
        {
            Context.Line = $"int64 {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(BoolType type)
        {
            Context.Line = $"bool {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(StringType type)
        {
            Context.Line = $"string {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(FloatType type)
        {
            Context.Line = $"float32 {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(DoubleType type)
        {
            Context.Line = $"float64 {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(ListType type)
        {
            var valVisitor = new ProtoTypeNameTypeVisitor();
            type.Value().Accept(valVisitor);
            Context.Line = $"repeated {valVisitor.Context.Name} {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }

        public override void Visit(MapType type)
        {
            var keyVisitor = new ProtoTypeNameTypeVisitor();
            type.Key().Accept(keyVisitor);
            var valVisitor = new ProtoTypeNameTypeVisitor();
            type.Value().Accept(valVisitor);
            Context.Line = $"map<{keyVisitor.Context.Name}, {valVisitor.Context.Name}> {FieldName} = {FieldIndex}; // {m_Field.Comment}";
        }
        
    }
}