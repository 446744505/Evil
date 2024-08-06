using System;
using Generator.Context;
using Generator.Kind;
using Generator.Type;

namespace Generator.Visitor
{
    public class EdbFullNameTypeVisitor : ITypeVisitor
    {
        private readonly Func<string, BaseIdentiferKind>? m_IdentiferFind;
        public EdbFullNameTypeVisitor(Func<string, BaseIdentiferKind>? finder = null)
        {
            m_IdentiferFind = finder;
        }

        public string Result { get; private set; } = null!;
        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            if (m_IdentiferFind == null)
                Result = type.Name;
            else
                Result = m_IdentiferFind.Invoke(type.Name).FullName();
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
            var valueVisitor = new EdbFullNameTypeVisitor(m_IdentiferFind);
            type.Value().Accept(valueVisitor);
            Result = $"System.Collections.Generic.IList<{valueVisitor.Result}>";
        }

        public void Visit(MapType type)
        {
            var keyVisitor = new EdbFullNameTypeVisitor(m_IdentiferFind);
            var valueVisitor = new EdbFullNameTypeVisitor(m_IdentiferFind);
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