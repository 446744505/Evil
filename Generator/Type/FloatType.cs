
using Generator.Visitor;

namespace Generator.Type
{
    public class FloatType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}