using Evil.Util;
using Generator.Context;
using Generator.Kind;

namespace Generator.Edb
{
    public class XTableGenerator : BaseGenerator<XTableNamespaceKind>
    {
        public XTableGenerator(GloableContext gc) : 
            base(gc, new GeneratorContext(), Files.XTablePath)
        {
        }

        protected override void Generate0()
        {
            foreach (var namespaceClassKind in NamespaceClassKinds())
            {
                var identiferKinds = namespaceClassKind.Value;
                foreach (var identiferKind in identiferKinds)
                {
                    Log.I.Info(identiferKind.Name);
                }
            }
        }
    }
}