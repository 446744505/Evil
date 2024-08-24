using Generator.Context;
using Generator.Type;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Visitor
{
    /// <summary>
    /// 1、根据ProtoReq方法的返回值类型生成对应的Ack类型
    /// 2、如果是IdentiferType，那么将其设置为Message类型(其实就是加入到全局ctx中)
    /// </summary>
    public class ProtoAckTypeVisitor : ITypeVisitor
    {
        /// <summary>
        /// ack对应的请求的名字
        /// </summary>
        private readonly string m_ReqName;
        private readonly GloableContext m_Gc;
        private string AckName => $"{m_ReqName}Ack";
        public TypeDeclarationSyntax? SyntaxResult { get; set; }
        public IType? TypeResult { get; set; }

        public ProtoAckTypeVisitor(string reqName, GloableContext gc)
        {
            m_ReqName = reqName;
            m_Gc = gc;
        }

        public void Visit(StructType type)
        {
            // 1、本身就有proto定义，不用生成
            // 2、设置为Message类型
            m_Gc.AddProtocolMessageName(type.Name);
        }

        public void Visit(ClassType type)
        {
            // 1、本身就有proto定义，不用生成
            // 2、设置为Message类型
            m_Gc.AddProtocolMessageName(type.Name);
        }

        public void Visit(ByteType type)
        {
            Visit0(type);
        }

        public void Visit(UShortType type)
        {
            Visit0(type);
        }

        private void Visit0(IType type)
        {
            TypeResult = type;
            SyntaxResult = SyntaxFactory.TypeDeclaration(SyntaxKind.ClassDeclaration, AckName);
        }

        public void Visit(IntType type)
        {
            Visit0(type);
        }

        public void Visit(UIntType type)
        {
            Visit0(type);
        }

        public void Visit(LongType type)
        {
            Visit0(type);
        }

        public void Visit(BoolType type)
        {
            Visit0(type);
        }

        public void Visit(StringType type)
        {
            Visit0(type);
        }

        public void Visit(FloatType type)
        {
            Visit0(type);
        }

        public void Visit(DoubleType type)
        {
            Visit0(type);
        }

        public void Visit(ArrayType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ListType type)
        {
            Visit0(type);
        }

        public void Visit(MapType type)
        {
            Visit0(type);
        }

        public void Visit(TaskType type)
        {
            var valueType = type.Value();
            valueType.Accept(this);
        }

        public void Visit(WaitCompileIdentiferType type)
        {
            m_Gc.AddProtocolMessageName(type.Name);
        }
    }
}