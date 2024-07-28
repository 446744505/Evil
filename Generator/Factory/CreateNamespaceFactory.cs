using Generator.Kind;

namespace Generator.Factory
{
    public interface ICreateNamespaceFactory
    {
        public NamespaceKind CreateNamespace(string name);
    }
    
    public class DefaultCreateNamespaceFactory : ICreateNamespaceFactory
    {
        public NamespaceKind CreateNamespace(string name)
        {
            return new NamespaceKind(name);
        }
    }
}