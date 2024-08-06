using System;
using Generator.Kind;
using Generator.Type;

namespace Generator.Visitor
{
    public class FullNameTypeVisitor : ITypeVisitor
    {
        private readonly Func<string, BaseIdentiferKind>? m_IdentiferFind;
        public FullNameTypeVisitor(Func<string, BaseIdentiferKind>? finder = null)
        {
            m_IdentiferFind = finder;
        }

        public string Result { get; private set; } = null!;

        private void VisitRef(BaseIdentiferType type)
        {
            if (m_IdentiferFind == null)
                Result = type.Name;
            else
                Result = m_IdentiferFind.Invoke(type.Name).FullName();
        }
        public void Visit(StructType type)
        {
            VisitRef(type);
        }

        public void Visit(ClassType type)
        {
            VisitRef(type);
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
            var valueVisitor = new FullNameTypeVisitor(m_IdentiferFind);
            type.Value().Accept(valueVisitor);
            Result = $"System.Collections.Generic.List<{valueVisitor.Result}>";
        }

        public void Visit(MapType type)
        {
            var keyVisitor = new FullNameTypeVisitor(m_IdentiferFind);
            var valueVisitor = new FullNameTypeVisitor(m_IdentiferFind);
            type.Key().Accept(keyVisitor);
            type.Value().Accept(valueVisitor);
            Result = $"System.Collections.Generic.Dictionary<{keyVisitor.Result}, {valueVisitor.Result}>";
        }

        public void Visit(TaskType type)
        {
            // 目前的需求都是Task返回内部类型
            var valueVisitor = new FullNameTypeVisitor();
            type.Value().Accept(valueVisitor);
            Result = valueVisitor.Result;
        }

        public void Visit(WaitCompileIdentiferType type)
        {
            Result = type.Name;
        }
    }
}