using Generator.Type;

namespace Generator.Visitor
{
    public interface ITypeVisitorContext
    {
        
    }
    public interface ITypeVisitor<T> where T : ITypeVisitorContext
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
    }
    
    public abstract class BaseTypeVisitor<T> : ITypeVisitor<T> where T : ITypeVisitorContext
    {
        private readonly T m_Context;
        public T Context => m_Context;
        
        public BaseTypeVisitor(T context)
        {
            m_Context = context;
        }

        public abstract void Visit(StructType type);
        public abstract void Visit(ClassType type);
        public abstract void Visit(IntType type);
        public abstract void Visit(LongType type);
        public abstract void Visit(BoolType type);
        public abstract void Visit(StringType type);
        public abstract void Visit(FloatType type);
        public abstract void Visit(DoubleType type);
        public abstract void Visit(ListType type);
        public abstract void Visit(MapType type);
    }
}