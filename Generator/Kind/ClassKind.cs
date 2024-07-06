using Generator.Type;

namespace Generator.Kind
{
    public class ClassKind : BaseIdentiferKind
    {
        public ClassKind(BaseIdentiferType type, IKind parent) : base(type, parent)
        {
        }

        public override BaseIdentiferType CreateIdentiferType()
        {
            return new ClassType(Name);
        }
    }
}