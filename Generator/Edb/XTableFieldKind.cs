using Generator.Type;

namespace Generator.Kind
{
    public class XTableFieldKind : EdbFieldKind
    {
        public XTableFieldKind(string name, IType type, IKind parent) : base(name, type, parent)
        {
        }
    }
}