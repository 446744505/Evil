using Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class ClientToServerAttrHandler : BaseTypeAttrHandler, ICreateSyntaxAttrHandler
    {
        private ICreateSyntaxAttrHandler m_CreateSyntaxAttrHandler = new DefaultCreateSyntaxAttrHandler();
        /// <summary>
        /// 解析类型里的所有方法，并生成对应的方法
        /// 会为每个方法生成一个协议
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected override void Parse0(TypeContext tc, AttributeSyntax attr)
        {
            var methodList = tc.TypeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var m in methodList)
            {
                var body = SyntaxFactory.ParseStatement("");
                var method = SyntaxFactory.MethodDeclaration(m.ReturnType, m.Identifier)
                    .WithBody(SyntaxFactory.Block(body));
                
                // 拷贝修饰符
                method = method.AddModifiers(m.Modifiers.ToArray());
                // 拷贝注释
                method = method.WithLeadingTrivia(m.GetLeadingTrivia());
                // 用来检查协议字段的索引是否重复
                HashSet<string> fieldIndex = new();
                // 拷贝参数,去掉参数注解
                foreach (var p in m.ParameterList.Parameters)
                {
                    // 不是协议字段
                    if (!AnalysisUtil.HadAttribute(p, Attributes.ProtocolField, out attr!))
                    {
                        throw new AttributeException($"{tc.ClassName}方法{m.Identifier}的参数{p.Identifier}必须要{Attributes.ProtocolField}注解");
                    }
                    // 检查是否有index字段
                    if (!AnalysisUtil.HadAttrArgument(attr, AttributeFields.ProtocolFieldIndex, out var index))
                    {
                        throw new AttributeException(
                            $"{tc.ClassName}方法{m.Identifier}的参数{p.Identifier}的{Attributes.ProtocolField}注解取不到index={AttributeFields.ProtocolFieldIndex}的字段");
                    }
                    // 检查索引是否重复
                    if (!fieldIndex.Add(index))
                    {
                        throw new AttributeException($"{tc.ClassName}方法{m.Identifier}的参数{p.Identifier}的索引{index}重复");
                    }
                
                    var param = SyntaxFactory.Parameter(p.Identifier)
                        .WithType(p.Type);
                    method = method.AddParameterListParameters(param);
                }

                tc.ClassSyntax = tc.ClassSyntax!.AddMembers(method);
            }
        }

        public override string GetAttrName()
        {
            return Attributes.ClientToServer;
        }

        public void InitSyntax(TypeContext tc, AttributeSyntax attr)
        {
            m_CreateSyntaxAttrHandler.InitSyntax(tc, attr);
        }

        public void FinishSyntax()
        {
            m_CreateSyntaxAttrHandler.FinishSyntax();
        }
    }
}