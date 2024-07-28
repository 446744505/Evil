using Generator.Factory;
using Generator.Kind;
using Generator.Type;

namespace Generator.Edb;

public class XTableCreateIdentiferFactory : ICreateIdentiferFactory
{
    public BaseIdentiferKind CreateIdentifer(BaseIdentiferType identiferType, IKind parent)
    {
        switch (identiferType)
        {
            case ClassType:
                return new XTableClassKind(identiferType, parent);
        }

        throw new System.NotSupportedException(identiferType.GetType().Name);
    }
}