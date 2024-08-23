
using Generator.Visitor;

namespace Generator.Type
{
    public class UIntType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}