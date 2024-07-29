using Generator.Factory;
using Generator.Kind;
using Generator.Type;

namespace Generator.Edb;

public class XBeanCreateIdentiferFactory : ICreateIdentiferFactory
{
    public BaseIdentiferKind CreateIdentifer(BaseIdentiferType identiferType, IKind parent)
    {
        switch (identiferType)
        {
            case ClassType:
                return new XBeanClassKind(identiferType, parent);
        }

        throw new System.NotSupportedException(identiferType.GetType().Name);
    }
}