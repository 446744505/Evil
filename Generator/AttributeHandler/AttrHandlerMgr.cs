using System;
using Generator.Util;

namespace Generator.AttributeHandler
{
    public class AttrHandlerMgr : Singleton<AttrHandlerMgr>
    {
        public IAttributeHandler CreateHandler(string attrName)
        {
            return attrName switch
            {
                Attributes.ClientToServer => new ClientToServerAttrHandler(),
                Attributes.Protocol => new ProtocolAttrHandler(),
                Attributes.ProtocolField => new ProtocolFieldAttrHandler(),
                _ => throw new NullReferenceException($"特性 {attrName} 没有对应的处理器")
            };
        }
    }
}