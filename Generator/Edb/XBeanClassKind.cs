
using Generator.Type;

namespace Generator.Kind
{
    public class XBeanClassKind : ClassKind
    {
        // 有值则表示这个table的bean
        public string IdFieldName { get; set; }
        public XBeanClassKind(BaseIdentiferType type, IKind parent) : base(type, parent)
        {
        }
    }
}