using Attributes;

namespace Evil.Switcher.Provide
{
    [Protocol(Node.Switcher)]
    public class BindProvide
    {
        [ProtocolField(1)] 
        private ProvideInfo info;
    }
}