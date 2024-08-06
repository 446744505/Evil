using Generator.Type;

namespace Generator.Kind
{
    public class XTableFieldKind : EdbFieldKind
    {
        public bool IsListenerField { get; set; }
        public XTableFieldKind(string name, IType type, IKind parent) : base(name, type, parent)
        {
        }
    }
}