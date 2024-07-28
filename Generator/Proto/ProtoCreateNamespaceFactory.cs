using Generator.Kind;

namespace Generator.Factory
{
    public class ProtoCreateNamespaceFactory : ICreateNamespaceFactory
    {
        public NamespaceKind CreateNamespace(string name)
        {
            return new ProtoNamespaceKind(name);
        }
    }
}