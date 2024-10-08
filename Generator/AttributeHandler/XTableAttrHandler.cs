﻿using System.Linq;
using Generator.Context;
using Generator.Edb;
using Generator.Exception;
using Generator.Factory;
using Generator.Kind;
using Generator.Type;
using Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public class XTableAttrHandler : BaseTypeAttrHandler, ICreateKindAttrHandler
    {
        private readonly ICreateKindAttrHandler m_CreateKindAttrHandler;
        private readonly ICreateNamespaceFactory m_XBeanCreateNamespaceFactory = new XBeanCreateNamespaceFactory();
        private readonly ICreateIdentiferFactory m_XBeanCreateIdentiferFactory = new XBeanCreateIdentiferFactory();
        private readonly ICreateFieldFactory<XBeanFieldKind> m_XBeanCreateFieldFactory = new XBeanCreateFieldFactory();
        
        public XTableAttrHandler(TypeContext typeContext, AttributeSyntax attr) : base(typeContext, attr)
        {
            m_CreateKindAttrHandler = new DefaultCreateKindAttrHandler()
            {
                ForceNamespace = Namespaces.XTableNamespace,
                CreateNamespaceFactory = new XTableCreateNamespaceFactory(),
                CreateIdentiferFactory = new XTableCreateIdentiferFactory(),
                CreateFieldFactory = new XTableCreateFieldFactory()
            };
            AnalysisUtil.HadAttrArgument(attr, AttributeFields.XTableNodes, out var nodes);
            NeedParse = TypeContext.FileContext.GloableContext.IsNodeAt(nodes);
        }
        
        protected override void Parse0()
        {
            // 获取capacity字段
            if (!AnalysisUtil.HadAttrArgument(Attr, AttributeFields.XTableCapacity, out var capacityStr))
            {
                throw new AttributeException(
                    $"表{TypeContext.OldClassName}的{Attributes.XTable}注解取不到index={AttributeFields.XTableCapacity}的字段");
            }
            var capacity = int.Parse(capacityStr);
            
            // 获取lockName字段
            if (!AnalysisUtil.HadAttrArgument(Attr, AttributeFields.XTableLock, out var lockName))
            {
                // 如果lockName字段没有设置，那么表名就是lockName
                lockName = TypeContext.OldClassName;
            }
            // 如果有.则取最后一部分
            if (lockName.Contains('.'))
            {
                lockName = lockName.Substring(lockName.LastIndexOf('.') + 1);
            }
            // 获取memory字段
            var isMemory = false;
            if (AnalysisUtil.HadAttrArgument(Attr, AttributeFields.XTableMemory, out var memoryStr))
            {
                isMemory = bool.Parse(memoryStr);
            }
            
            // 给table创建一个XBean
            var xBean = (XBeanClassKind)m_XBeanCreateIdentiferFactory.CreateIdentifer(new ClassType(TypeContext.OldClassName),
                TypeContext.FileContext.GetOrCreateNamespaceKind(Namespaces.XBeanNamespace, m_XBeanCreateNamespaceFactory));
            if (AnalysisUtil.HadAttribute(TypeContext.OldTypeSyntax, Attributes.Protocol, out _))
            {
                xBean.IsProtoField = true;
            }

            var idFieldName = string.Empty;
            var fields = TypeContext.OldTypeSyntax.DescendantNodes().OfType<FieldDeclarationSyntax>();
            foreach (var f in fields)
            {
                var fieldName = AnalysisUtil.GetFieldName(f);
                // 不是db列
                if (!AnalysisUtil.HadAttribute(f, Attributes.XColumn, out var attr))
                {
                    continue;
                }
                if (AnalysisUtil.HadAttrArgument(attr!, AttributeFields.XColumnId, out var idStr))
                {
                    var isId = bool.Parse(idStr);
                    if (isId)
                    {
                        if (!string.IsNullOrEmpty(idFieldName))
                        {
                            throw new AttributeException($"表{TypeContext.OldClassName}的有多个id字段{fieldName} {idFieldName}");
                        }
                        idFieldName = fieldName;
                    }
                }

                // 给xtable加字段
                var tableField = (XTableFieldKind)NewField(NewFieldContext.Parse(f));
                if (AnalysisUtil.HadAttribute(f, Attributes.XListener, out _))
                {
                    tableField.IsListenerField = true;
                }
                
                // 给xbean加字段
                var xBeanField = m_XBeanCreateFieldFactory.CreateField(NewFieldContext.Parse(f), xBean);
                if (AnalysisUtil.HadAttribute(f, Attributes.ProtocolField, out _))
                {
                    xBeanField.IsProtoField = true;
                }
                xBean.AddField(xBeanField);
            }
            if (string.IsNullOrEmpty(idFieldName))
            {
                throw new AttributeException($"表{TypeContext.OldClassName}没有id字段");
            }
            
            var tableBean = (XBeanClassKind)xBean;
            tableBean.IdFieldName = idFieldName;

            var tableKind = TypeContext.IdentiferKind as XTableClassKind;
            tableKind!.Capacity = capacity;
            tableKind.LockName = lockName;
            tableKind.IsMemory = isMemory;
            tableKind.IdFieldName = idFieldName;
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