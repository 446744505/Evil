
using Generator.Visitor;

namespace Generator.Type
{
    public class StringType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}