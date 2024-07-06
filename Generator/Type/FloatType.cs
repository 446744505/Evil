
namespace Generator
{
    public class FloatType : BaseBaseType
    {
        public override void Accept<T>(ITypeVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}