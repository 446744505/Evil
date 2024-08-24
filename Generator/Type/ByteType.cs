
using Generator.Visitor;

namespace Generator.Type
{
    public class ByteType : BaseBaseType
    {
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}