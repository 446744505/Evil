namespace Generator
{
    public class ProtoTypeVisitorContext : ITypeVisitorContext
    {
        public string Line { get; set; }
    }
    public class ProtoTypeVisitor : BaseTypeVisitor<ProtoTypeVisitorContext>
    {
        public ProtoTypeVisitor() : base(new ProtoTypeVisitorContext())
        {
        }
        
        public override void Visit(StructType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ClassType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IntType type)
        {
            Context.Line = "int32";
        }

        public override void Visit(LongType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BoolType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(StringType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FloatType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DoubleType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ListType type)
        {
            throw new NotImplementedException();
        }

        public override void Visit(MapType type)
        {
            throw new NotImplementedException();
        }
        
    }
}