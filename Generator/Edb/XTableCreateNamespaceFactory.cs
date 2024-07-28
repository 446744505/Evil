using Generator.Kind;

namespace Generator.Factory
{
    public class XTableCreateNamespaceFactory : ICreateNamespaceFactory
    {
        public NamespaceKind CreateNamespace(string name)
        {
            return new XTableNamespaceKind(name);
        }
    }
}