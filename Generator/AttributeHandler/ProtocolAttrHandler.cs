using System.Collections.Generic;
using System.Linq;
using Generator.Context;
using Generator.Exception;
using Generator.Factory;
using Generator.Kind;
using Generator.Type;
using Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    /// <summary>
    /// 解析所有协议字段构造类型信息，用于后面生成.proto文件
    /// </summary>
    public class ProtocolAttrHandler : BaseTypeAttrHandler, ICreateKindAttrHandler
    {
        private readonly ICreateKindAttrHandler m_CreateKindAttrHandler;
        
        public ProtocolAttrHandler(TypeContext typeContext, AttributeSyntax attr) : base(typeContext, attr)
        {
            m_CreateKindAttrHandler = new DefaultCreateKindAttrHandler()
            {
                CreateNamespaceFactory = new ProtoCreateNamespaceFactory(),
                CreateIdentiferFactory = new ProtoCreateIdentiferFactory(),
                CreateFieldFactory = new ProtoCreateFieldFactory(),
                ForceNamespace = Namespaces.ProtoNamespace
            };
            AnalysisUtil.HadAttrArgument(attr, AttributeFields.ProtocolNodes, out var nodes);
            NeedParse = TypeContext.FileContext.GloableContext.IsNodeAt(nodes);
        }

        protected override void Parse0()
        {
            // 消息最大字节处理
            AnalysisUtil.HadAttrArgument(Attr, AttributeFields.ProtocolMaxSize, out var maxSize);
            if (!string.IsNullOrEmpty(maxSize))
                ((ProtoClassKind)TypeContext.IdentiferKind!).MaxSize = int.Parse(maxSize);
            
            var fields = TypeContext.OldTypeSyntax.DescendantNodes().OfType<FieldDeclarationSyntax>();
            // 用来检查协议字段的索引是否重复
            HashSet<string> fieldIndex = new();
            foreach (var f in fields)
            {
                // 不是协议字段
                if (!AnalysisUtil.HadAttribute(f, Attributes.ProtocolField, out var attr))
                {
                    // 如果是常量
                    if (f.Modifiers.Any(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ConstKeyword)))
                    {
                        var ctx = NewFieldContext.Parse(f);
                        var field = ConstFieldKind.NewConstFieldKind(ctx, TypeContext.IdentiferKind!);
                        field.Define = f.ToString();
                    }
                    continue;
                }
                var fieldName = AnalysisUtil.GetFieldName(f);
                // 检查是否有index字段
                if (!AnalysisUtil.HadAttrArgument(attr!, AttributeFields.ProtocolFieldIndex, out var index))
                {
                    throw new AttributeException(
                        $"{TypeContext.OldClassName}的字段{fieldName}的{Attributes.ProtocolField}注解取不到index={AttributeFields.ProtocolFieldIndex}的字段");
                }
                // 检查索引是否重复
                if (!fieldIndex.Add(index))
                {
                    throw new AttributeException($"{TypeContext.OldClassName}的字段{fieldName}的索引{index}重复");
                }
                // 获取字段类型
                try
                {
                    var ctx = NewFieldContext.Parse(f);
                    var field = NewField(ctx);
                    var protoField = field as ProtoFieldKind;
                    protoField!.Index = int.Parse(index);
                }
                catch (System.Exception e)
                {
                    throw new TypeException($"{TypeContext.OldClassName}的字段{fieldName}的类型解析失败", e);
                }
            }
            // 设置为Message类型
            TypeContext.FileContext.GloableContext.AddProtocolMessageName(TypeContext.OldClassName, false);
        }

        public ICreateNamespaceFactory CreateNamespaceFactory => m_CreateKindAttrHandler.CreateNamespaceFactory;

        public void InitKind(TypeContext tc, AttributeSyntax attr)
        {
            m_CreateKindAttrHandler.InitKind(tc, attr);
        }

        public FieldKind NewField(NewFieldContext ctx)
        {
            return m_CreateKindAttrHandler.NewField(ctx);
        }
    }
}