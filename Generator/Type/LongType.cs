
namespace Generator
{
    public class LongType : BaseBaseType
    {
        public override void Accept<T>(ITypeVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}