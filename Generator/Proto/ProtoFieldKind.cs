using Generator.Type;

namespace Generator.Kind
{
    public class ProtoFieldKind : FieldKind
    {
        private int m_Index;
        public int Index
        {
            get => m_Index;
            set => m_Index = value;
        }

        public ProtoFieldKind(string name, IType type, IKind parent) 
            : base(name, type, parent)
        {
        }
    }
}