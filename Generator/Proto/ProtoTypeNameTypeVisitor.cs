
using Generator.Context;
using Generator.Proto;
using Generator.Type;

namespace Generator.Visitor
{
    /// <summary>
    /// 计算生成proto文件时字段的类型定义
    /// </summary>
    public class ProtoTypeNameTypeVisitor : ITypeVisitor
    {
        private readonly GeneratorContext m_Pc;
        public string Result { get; set; }
        public ProtoTypeNameTypeVisitor(GeneratorContext pc)
        {
            m_Pc = pc;
        }
        
        private void VisitIdentiferType(BaseIdentiferType identiferType)
        {
            var identiferKind = m_Pc.IdentiferFind.Invoke(identiferType.Name);
            Result = $"{identiferKind.NamespaceName()}.{identiferKind.Name}";
        }

        public void Visit(StructType type)
        {
            VisitIdentiferType(type);
        }

        public void Visit(ClassType type)
        {
            VisitIdentiferType(type);
        }

        public void Visit(IntType type)
        {
            Result = "int32";
        }

        public void Visit(LongType type)
        {
            Result = "int64";
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
            Result = "float32";
        }

        public void Visit(DoubleType type)
        {
            Result = "float64";
        }

        public void Visit(ListType type)
        {
            var valVisitor = new ProtoTypeNameTypeVisitor(m_Pc);
            type.Value().Accept(valVisitor);
            Result = $"repeated {valVisitor.Result}";
        }

        public void Visit(MapType type)
        {
            var keyVisitor = new ProtoTypeNameTypeVisitor(m_Pc);
            type.Key().Accept(keyVisitor);
            var valVisitor = new ProtoTypeNameTypeVisitor(m_Pc);
            type.Value().Accept(valVisitor);
            Result = $"map<{keyVisitor.Result}, {valVisitor.Result}>";
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