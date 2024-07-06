
using Generator.Visitor;

namespace Generator.Type
{
    public class BoolType : BaseBaseType
    {
        public override void Accept<T>(ITypeVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}