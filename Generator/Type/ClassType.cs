using Generator.Context;
using Generator.Kind;
using Generator.Visitor;

namespace Generator.Type
{
    public class ClassType : BaseIdentiferType
    {
        public ClassType()
        {
        }
        public ClassType(string name)
        {
            Name = name;
        }
        
        public override ClassType Compile(CompileContext ctx)
        {
            return this;
        }

        public override BaseIdentiferKind CreateKind(IKind parent)
        {
            return new ClassKind(this, parent);
        }
        
        public override void Accept(ITypeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}