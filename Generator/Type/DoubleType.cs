
using Generator.Visitor;

namespace Generator.Type
{
    public class DoubleType : BaseBaseType
    {
        public override void Accept<T>(ITypeVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}