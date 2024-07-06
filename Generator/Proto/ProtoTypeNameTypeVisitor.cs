
using Generator.Type;

namespace Generator.Visitor
{
    public class ProtoTypeNameTypeVisitorContext : ITypeVisitorContext
    {
        public string Name { get; set; }
    }
    public class ProtoTypeNameTypeVisitor : BaseTypeVisitor<ProtoTypeNameTypeVisitorContext>
    {
        public ProtoTypeNameTypeVisitor() : base(new ProtoTypeNameTypeVisitorContext())
        {
        }

        public override void Visit(StructType type)
        {
            Context.Name = type.Name;
        }

        public override void Visit(ClassType type)
        {
            Context.Name = type.Name;
        }

        public override void Visit(IntType type)
        {
            Context.Name = "int32";
        }

        public override void Visit(LongType type)
        {
            Context.Name = "int64";
        }

        public override void Visit(BoolType type)
        {
            Context.Name = "bool";
        }

        public override void Visit(StringType type)
        {
            Context.Name = "string";
        }

        public override void Visit(FloatType type)
        {
            Context.Name = "float32";
        }

        public override void Visit(DoubleType type)
        {
            Context.Name = "float64";
        }

        public override void Visit(ListType type)
        {
            var valVisitor = new ProtoTypeNameTypeVisitor();
            type.Value().Accept(valVisitor);
            Context.Name = $"repeated {valVisitor.Context.Name}";
        }

        public override void Visit(MapType type)
        {
            var keyVisitor = new ProtoTypeNameTypeVisitor();
            type.Key().Accept(keyVisitor);
            var valVisitor = new ProtoTypeNameTypeVisitor();
            type.Value().Accept(valVisitor);
            Context.Name = $"map<{keyVisitor.Context.Name}, {valVisitor.Context.Name}>";
        }
    }
}