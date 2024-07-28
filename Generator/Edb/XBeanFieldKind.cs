using Generator.Type;

namespace Generator.Kind
{
    public class XBeanFieldKind : EdbFieldKind
    {
        public XBeanFieldKind(string name, IType type, IKind parent) : base(name, type, parent)
        {
        }
    }
}