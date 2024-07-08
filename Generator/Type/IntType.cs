
using Generator.Visitor;

namespace Generator.Type
{
    public class IntType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}