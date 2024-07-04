namespace Generator
{
    public class AttrHandlerMgr
    {
        public static readonly AttrHandlerMgr I = new();

        private AttrHandlerMgr()
        {
            RegisterAttrHandler(new ClientToServerAttrHandler());
            RegisterAttrHandler(new ProtocolAttrHandler());
            RegisterAttrHandler(new ProtocolFieldAttrHandler());
        }

        private readonly Dictionary<string, IAttributeHandler> _handlers = new();

        private void RegisterAttrHandler(IAttributeHandler handler)
        {
            _handlers[handler.GetAttrName()] = handler;
        }
        
        public IAttributeHandler? GetHandler(string attrName)
        {
            return _handlers[attrName];
        }
    }
}