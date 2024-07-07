using Generator.Type;
using Generator.Visitor;

namespace Generator.Proto
{
    public class ProtoImportTypeVisitorContext : ITypeVisitorContext
    {
        public string Line { get; set; }
    }
    public class ProtoImportTypeVisitor : BaseTypeVisitor<ProtoImportTypeVisitorContext>
    {
        public ProtoImportTypeVisitor() : base(new ProtoImportTypeVisitorContext())
        {
        }

        public override void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(ClassType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(IntType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(LongType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(BoolType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(StringType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(FloatType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DoubleType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(ListType type)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(MapType type)
        {
            throw new System.NotImplementedException();
        }
    }
}