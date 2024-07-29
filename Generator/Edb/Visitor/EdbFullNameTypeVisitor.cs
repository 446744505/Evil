using Generator.Type;

namespace Generator.Visitor
{
    public class EdbFullNameTypeVisitor : ITypeVisitor
    {
        public string Result { get; private set; }
        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            Result = type.Name;
        }

        public void Visit(IntType type)
        {
            Result = "int";
        }

        public void Visit(LongType type)
        {
            Result = "long";
        }

        public void Visit(BoolType type)
        {
            Result = "bool";
        }

        public void Visit(StringType type)
        {
            Result = "string";
        }

        public void Visit(FloatType type)
        {
            Result = "float";
        }

        public void Visit(DoubleType type)
        {
            Result = "double";
        }

        public void Visit(ListType type)
        {
            var valueVisitor = new FullNameTypeVisitor();
            type.Value().Accept(valueVisitor);
            Result = $"System.Collections.Generic.IList<{valueVisitor.Result}>";
        }

        public void Visit(MapType type)
        {
            var keyVisitor = new FullNameTypeVisitor();
            var valueVisitor = new FullNameTypeVisitor();
            type.Key().Accept(keyVisitor);
            type.Value().Accept(valueVisitor);
            Result = $"System.Collections.Generic.IDictionary<{keyVisitor.Result}, {valueVisitor.Result}>";
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