namespace Generator.AttributeHandler
{
    public class AttrHandlerMgr
    {
        public static readonly AttrHandlerMgr I = new();

        private AttrHandlerMgr()
        {
        }

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