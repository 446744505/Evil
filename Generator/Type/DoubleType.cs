
namespace Generator
{
    public class DoubleType : BaseBaseType
    {
        public override void Accept<T>(ITypeVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}