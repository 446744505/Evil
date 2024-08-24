using Generator.Type;

namespace Generator.Visitor
{
    public class MongoAsTypeVisitor : ITypeVisitor
    {
        public string Result { get; private set; }
        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ByteType type)
        {
            Result = "AsInt32";
        }

        public void Visit(UShortType type)
        {
            Result = "AsInt32";
        }

        public void Visit(IntType type)
        {
            Result = "AsInt32";
        }

        public void Visit(UIntType type)
        {
            Result = "AsInt32";
        }

        public void Visit(LongType type)
        {
            Result = "AsInt64";
        }

        public void Visit(BoolType type)
        {
            Result = "AsBoolean";
        }

        public void Visit(StringType type)
        {
            Result = "AsString";
        }

        public void Visit(FloatType type)
        {
            throw new System.NotSupportedException("MongoDB does not support float type");
        }

        public void Visit(DoubleType type)
        {
            Result = "AsDouble";
        }

        public void Visit(ArrayType type)
        {
            Result = "AsByteArray";
        }

        public void Visit(ListType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MapType type)
        {
            throw new System.NotImplementedException();
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