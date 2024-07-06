using Generator.Context;

namespace Generator.Kind
{
    public class NamespaceKind : BaseKind
    {
        private readonly string m_Name;
    
        public string Name => m_Name;
    
        public NamespaceKind(string name) : base(null!)
        {
            m_Name = name;
        }
    
        public new List<BaseIdentiferKind> Children()
        {
            return base.Children().Cast<BaseIdentiferKind>().ToList();
        }

        protected override void Compile0(CompileContext ctx)
        {
        
        }
    }
}