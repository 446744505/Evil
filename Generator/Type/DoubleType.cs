
using Generator.Visitor;

namespace Generator.Type
{
    public class DoubleType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}