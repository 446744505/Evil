
using Generator.Visitor;

namespace Generator.Type
{
    public class BoolType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}