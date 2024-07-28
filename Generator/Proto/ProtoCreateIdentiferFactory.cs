using Generator.Kind;
using Generator.Type;

namespace Generator.Factory
{
    public class ProtoCreateIdentiferFactory : ICreateIdentiferFactory
    {
        public BaseIdentiferKind CreateIdentifer(BaseIdentiferType identiferType, IKind parent)
        {
            switch (identiferType)
            {
                case ClassType:
                    return new ProtoClassKind(identiferType, parent);
            }

            throw new System.NotSupportedException(identiferType.GetType().Name);
        }
    }
}