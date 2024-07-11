using Generator.Type;

namespace Generator.Visitor
{
    public interface ITypeVisitor
    {
        public void Visit(StructType type);
        public void Visit(ClassType type);
        public void Visit(IntType type);
        public void Visit(LongType type);
        public void Visit(BoolType type);
        public void Visit(StringType type);
        public void Visit(FloatType type);
        public void Visit(DoubleType type);
        public void Visit(ListType type);
        public void Visit(MapType type);
        public void Visit(TaskType type);
    }
}