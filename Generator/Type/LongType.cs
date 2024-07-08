
using Generator.Visitor;

namespace Generator.Type
{
    public class LongType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}