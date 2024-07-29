using Generator.Kind;

namespace Generator.Factory
{
    public class XBeanCreateNamespaceFactory : ICreateNamespaceFactory
    {
        public NamespaceKind CreateNamespace(string name)
        {
            return new XBeanNamespaceKind(name);
        }
    }
}