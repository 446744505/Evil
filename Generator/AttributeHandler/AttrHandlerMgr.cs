using System;
using Evil.Util;
using Generator.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public class AttrHandlerMgr : Singleton<AttrHandlerMgr>
    {
        public IAttributeHandler CreateHandler(string attrName, TypeContext tc, AttributeSyntax attr)
        {
            return attrName switch
            {
                Attributes.ClientToServer => new ClientToServerAttrHandler(tc, attr),
                Attributes.Protocol => new ProtocolAttrHandler(tc, attr),
                Attributes.ProtocolField => new ProtocolFieldAttrHandler(),
                Attributes.XTable => new XTableAttrHandler(tc, attr),
                Attributes.XBean => new XBeanAttrHandler(tc, attr),
                Attributes.XColumn => new XColumnAttrHandler(),
                _ => throw new NullReferenceException($"特性 {attrName} 没有对应的处理器")
            };
        }
    }
}