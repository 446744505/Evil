using Generator.Context;
using Generator.Kind;
using Generator.Visitor;

namespace Generator.Type
{
    public class StructType : BaseIdentiferType
    {
        public StructType()
        {
        }
        public StructType(string name)
        {
            Name = name;
        }
        public override StructType Compile(CompileContext ctx)
        {
            return this;
        }
        
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}