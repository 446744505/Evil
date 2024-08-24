
using Generator.Visitor;

namespace Generator.Type
{
    public class UShortType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}