using Generator.Type;

namespace Generator.Kind
{
    public class XTableClassKind : ClassKind
    {
        public string LockName { get; set; }
        public int Capacity { get; set; }
        public bool IsMemory { get; set; }
        public XTableClassKind(BaseIdentiferType type, IKind parent) : base(type, parent)
        {
        }
    }
}