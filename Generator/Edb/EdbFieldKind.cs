using Generator.Type;

namespace Generator.Kind
{
    public class EdbFieldKind : FieldKind
    {
        public EdbFieldKind(string name, IType type, IKind parent) : base(name, type, parent)
        {
        }
    }
}