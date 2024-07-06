using Generator.Kind;

namespace Generator
{
    public class ClassType : BaseIdentiferType
    {
        public ClassType()
        {
        }
        public ClassType(string name)
        {
            m_Name = name;
        }
        
        public override ClassType Compile(CompileContext ctx)
        {
            return this;
        }

        public override BaseIdentiferKind CreateKind(IKind parent)
        {
            return new ClassKind(this, parent);
        }
        
        public override void Accept<T>(ITypeVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}