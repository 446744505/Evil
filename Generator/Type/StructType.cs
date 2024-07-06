using Generator.Kind;

namespace Generator
{
    public class StructType : BaseIdentiferType
    {
        public StructType()
        {
        }
        public StructType(string name)
        {
            m_Name = name;
        }
        public override StructType Compile(CompileContext ctx)
        {
            return this;
        }

        public override BaseIdentiferKind CreateKind(IKind parent)
        {
            return new StructKind(this, parent);
        }
        
        public override void Accept<T>(ITypeVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}