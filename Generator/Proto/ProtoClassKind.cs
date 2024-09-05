using Generator.Type;

namespace Generator.Kind
{
    public class ProtoClassKind : ClassKind
    {
        public int MaxSize { get; set; } = -1;
        public ProtoClassKind(BaseIdentiferType type, IKind parent) : base(type, parent)
        {
        }
    }
}