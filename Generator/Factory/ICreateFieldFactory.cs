using Generator.Kind;
using Generator.Type;

namespace Generator.Factory
{
    public interface ICreateFieldFactory<out T> where T : FieldKind
    {
        public T CreateField(string name, IType type, IKind parent);
    }
    
    public class DefaultCreateFieldFactory : ICreateFieldFactory<FieldKind>
    {
        public FieldKind CreateField(string name, IType type, IKind parent)
        {
            return new FieldKind(name, type, parent);
        }
    }
}