using System.Collections.Generic;
using System.Linq;
using Generator.Context;
using Generator.Exception;
using Generator.Kind;
using Generator.Type;
using Generator.Util;
using Generator.Visitor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    class ClientToServerCreateSyntaxAttrHandler : DefaultCreateSyntaxAttrHandler
    {
        protected override void NewNamespaceSyntax()
        {
            var tc = m_TypeContext;
            // namespace添加Proto
            tc.NewNamespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName($"{tc.OldNameSpaceName}.{Namespaces.ProtoNamespace}"))
                .WithUsings(AnalysisUtil.SkipAttributes(tc.OldNameSpaceSyntax.Usings));
            tc.NewNamespaceSyntax = tc.NewNamespaceSyntax.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Client.NetWork")));
        }
    }
    public class ClientToServerAttrHandler : BaseTypeAttrHandler, ICreateSyntaxAttrHandler
    {
        private readonly ICreateSyntaxAttrHandler m_CreateSyntaxAttrHandler = new ClientToServerCreateSyntaxAttrHandler();
        /// <summary>
        /// 解析类型里的所有方法，并生成对应的方法
        /// 会为每个方法生成一个请求协议，如果方法有返回值且类型不是class/struct也会生成一个响应协议
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected override void Parse0(TypeContext tc, AttributeSyntax attr)
        {
            var methodList = tc.OldTypeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var m in methodList)
            {
                var method = ParseMethod(tc, m);
                tc.NewClassSyntax = tc.NewClassSyntax!.AddMembers(method);
            }
        }

        private MethodDeclarationSyntax ParseMethod(TypeContext tc, MethodDeclarationSyntax m)
        {
            var method = SyntaxFactory.MethodDeclaration(m.ReturnType, m.Identifier);
            var modifiers = m.Modifiers;
            var isReturnVoid = m.ReturnType.IsKind(SyntaxKind.VoidKeyword);
            // 如果返回值不是void,则设置为异步方法
            if (!isReturnVoid)
            {
                modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));
            }

            // 拷贝修饰符
            method = method.AddModifiers(modifiers.ToArray());
            // 拷贝注释
            method = method.WithLeadingTrivia(m.GetLeadingTrivia());
            // 用来检查协议字段的索引是否重复
            HashSet<string> fieldIndex = new();
            // 每个方法创建一个请求协议
            var reqClassKind = new ClassType().Parse(method)
                .CreateKind(tc.FileContext.GetOrCreateNamespaceKind(tc.OldNameSpaceName));
            reqClassKind.Comment = AnalysisUtil.GetComment(m);
            tc.FileContext.GloableContext.AddProtocolMessageName(reqClassKind.Name);
            var sendBody = new Writer();
            const string reqName = "req";
            // 可能会创建一个响应协议
            if (!isReturnVoid)
            {
                var returnType = TypeBuilder.I.ParseType(m.ReturnType);
                var protoAckVisitor = new ProtoAckTypeVisitor(reqClassKind.Name, tc.FileContext.GloableContext);
                returnType.Accept(protoAckVisitor);
                if (protoAckVisitor.SyntaxResult != null)
                {
                    var ackClassKind = new ClassType().Parse(protoAckVisitor.SyntaxResult)
                        .CreateKind(tc.FileContext.GetOrCreateNamespaceKind(tc.OldNameSpaceName));
                    ackClassKind.Comment = reqClassKind.Comment;
                    tc.FileContext.GloableContext.AddProtocolMessageName(ackClassKind.Name);
                    // 字段名永远是data
                    var field = new ProtoFieldKind(Fields.MessageAckFieldName, protoAckVisitor.TypeResult!, ackClassKind);
                    field.Index = 1; // index永远是1
                    reqClassKind.AddField(field);
                }
                // 异步发送
                var fullNameVisitor = new FullNameTypeVisitor();
                returnType.Accept(fullNameVisitor);
                sendBody.WriteLine($"return await Net.I.SendAsync<{fullNameVisitor.Result}>({reqName});");
            }
            else
            {
                // 直接发送
                sendBody.WriteLine($"Net.I.Send({reqName});");
            }
            // 生成方法体
            var body = new Writer();
            body.WriteLine($"var {reqName} = new {reqClassKind.Name}");
            body.WriteLine("{");

            // 拷贝参数,去掉参数注解
            foreach (var p in m.ParameterList.Parameters)
            {
                // 不是协议字段
                if (!AnalysisUtil.HadAttribute(p, Attributes.ProtocolField, out var attr))
                {
                    throw new AttributeException(
                        $"{tc.OldClassName}方法{m.Identifier}的参数{p.Identifier}必须要{Attributes.ProtocolField}注解");
                }

                // 检查是否有index字段
                if (!AnalysisUtil.HadAttrArgument(attr!, AttributeFields.ProtocolFieldIndex, out var index))
                {
                    throw new AttributeException(
                        $"{tc.OldClassName}方法{m.Identifier}的参数{p.Identifier}的{Attributes.ProtocolField}注解取不到index={AttributeFields.ProtocolFieldIndex}的字段");
                }

                // 检查索引是否重复
                if (!fieldIndex.Add(index))
                {
                    throw new AttributeException($"{tc.OldClassName}方法{m.Identifier}的参数{p.Identifier}的索引{index}重复");
                }

                var param = SyntaxFactory.Parameter(p.Identifier)
                    .WithType(p.Type);
                method = method.AddParameterListParameters(param);
                // 添加协议字段
                var field = new ProtoFieldKind(p.Identifier.Text, TypeBuilder.I.ParseType(p.Type!), reqClassKind);
                field.Index = int.Parse(index);
                reqClassKind.AddField(field);
                // 添加方法体赋值
                body.WriteLine(1, $"{p.Identifier} = {p.Identifier},");
            }
            // 方法体结束部分
            body.WriteLine("};");
            body.WriteLine(sendBody.ToString());
            method = method.WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement(body.ToString())));
            return method;
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