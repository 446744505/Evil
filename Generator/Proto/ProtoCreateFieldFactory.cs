using Generator.Kind;
using Generator.Type;

namespace Generator.Factory
{
    public class ProtoCreateFieldFactory : ICreateFieldFactory<ProtoFieldKind>
    {
        public ProtoFieldKind CreateField(string name, IType type, IKind parent)
        {
            return new ProtoFieldKind(name, type, parent);
        }
    }
}