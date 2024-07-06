namespace Generator.Kind
{
    public class StructKind : BaseIdentiferKind
    {
        public StructKind(BaseIdentiferType type, IKind parent) : base(type, parent)
        {
        }

        public override BaseIdentiferType CreateIdentiferType()
        {
            return new StructType(Name);
        }
    }
}