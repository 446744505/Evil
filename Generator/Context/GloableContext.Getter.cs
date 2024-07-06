using Generator.Kind;

namespace Generator.Context
{
    public partial class GloableContext
    {
        public BaseIdentiferKind? FindIdentiferKind(string name)
        {
            foreach (var fc in FileContexts)
            {
                foreach (var namespaceKind in fc.NamespaceKinds)
                {
                    foreach (var classKind in namespaceKind.Children())
                    {
                        if (classKind.Name == name)
                        {
                            return classKind;
                        }
                    }
                }
            }

            return null;
        }
    }
}