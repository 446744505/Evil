using System.Collections.Generic;
using System.Linq;
using Generator.Context;

namespace Generator.Kind
{
    public class NamespaceKind : BaseKind
    {
        public string Name { get; }
    
        public NamespaceKind(string name) : base(null!)
        {
            Name = name;
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