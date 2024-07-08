using Generator.Type;

namespace Generator.Kind
{
    public class ProtoFieldKind : FieldKind
    {
        public int Index { get; set; }

        public ProtoFieldKind(string name, IType type, IKind parent) 
            : base(name, type, parent)
        {
        }
    }
}