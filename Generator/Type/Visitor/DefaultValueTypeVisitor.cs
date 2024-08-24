using Generator.Type;

namespace Generator.Visitor
{
    public class DefaultValueTypeVisitor : ITypeVisitor
    {
        private readonly bool m_Full;
        public string Result;

        public DefaultValueTypeVisitor(bool full = false)
        {
            m_Full = full;
        }

        public void Visit(StructType type)
        {
        }

        public void Visit(ClassType type)
        {
            if (m_Full)
                Result = $"new {type.Name}()";
            else
                Result = "new()";
        }

        public void Visit(ByteType type)
        {
        }

        public void Visit(UShortType type)
        {
        }

        public void Visit(IntType type)
        {
        }

        public void Visit(UIntType type)
        {
        }

        public void Visit(LongType type)
        {
        }

        public void Visit(BoolType type)
        {
        }

        public void Visit(StringType type)
        {
            Result = "string.Empty";
        }

        public void Visit(FloatType type)
        {
        }

        public void Visit(DoubleType type)
        {
        }

        public void Visit(ArrayType type)
        {
        }

        public void Visit(ListType type)
        {
            if (m_Full)
            {
                var valueVisitor = new FullNameTypeVisitor();
                type.Value().Accept(valueVisitor);
                Result = $"new System.Collections.Generic.List<{valueVisitor.Result}>()";
            }
            else
                Result = "new()";
        }

        public void Visit(MapType type)
        {
            if (m_Full)
            {
                var keyVisitor = new FullNameTypeVisitor();
                type.Key().Accept(keyVisitor);
                var valueVisitor = new FullNameTypeVisitor();
                type.Value().Accept(valueVisitor);
                Result = $"new System.Collections.Generic.Dictionary<{keyVisitor.Result}, {valueVisitor.Result}>()";
            }
            else
                Result = "new()";
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