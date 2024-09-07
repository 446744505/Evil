using Generator.Kind;
using Generator.Type;

namespace Generator.Visitor
{
    public class EdbDefaultValueTypeVisitor : ITypeVisitor
    {
        private readonly XBeanFieldKind m_FieldKind;

        /// <summary>
        /// 如果有值则ClassType生成的构造函数是复制构造函数
        /// </summary>
        private readonly string m_CopyClass;

        private string FieldName => m_FieldKind.Name;
        public string Result;

        public EdbDefaultValueTypeVisitor(XBeanFieldKind fieldKind, string copyClass = "")
        {
            m_FieldKind = fieldKind;
            m_CopyClass = copyClass;
        }

        public void Visit(StructType type)
        {
        }

        public void Visit(ClassType type)
        {
            if (string.IsNullOrEmpty(m_CopyClass))
                Result = $"new(this, \"{FieldName}\")";
            else
                Result = $"new({m_CopyClass}, this, \"{FieldName}\")";
        }

        private bool VisitBase()
        {
            if (!string.IsNullOrEmpty(m_CopyClass))
            {
                Result = m_CopyClass;
                return false;
            }
                
            return true;
        }

        public void Visit(ByteType type)
        {
            VisitBase();
        }

        public void Visit(UShortType type)
        {
            VisitBase();
        }

        public void Visit(IntType type)
        {
            VisitBase();
        }

        public void Visit(UIntType type)
        {
            VisitBase();
        }

        public void Visit(LongType type)
        {
            VisitBase();
        }

        public void Visit(BoolType type)
        {
            VisitBase();
        }

        public void Visit(StringType type)
        {
            if (VisitBase())
                Result = "string.Empty";
        }

        public void Visit(FloatType type)
        {
            VisitBase();
        }

        public void Visit(DoubleType type)
        {
            VisitBase();
        }

        public void Visit(ArrayType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ListType type)
        {
            var valueVisitor = new FullNameTypeVisitor();
            type.Value().Accept(valueVisitor);
            Result = $"new System.Collections.Generic.List<{valueVisitor.Result}>()";
        }

        public void Visit(MapType type)
        {
            var keyVisitor = new FullNameTypeVisitor();
            type.Key().Accept(keyVisitor);
            var valueVisitor = new FullNameTypeVisitor();
            type.Value().Accept(valueVisitor);
            Result = $"new System.Collections.Generic.Dictionary<{keyVisitor.Result}, {valueVisitor.Result}>()";
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