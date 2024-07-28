using System;
using Generator.Kind;
using Generator.Type;

namespace Generator.Factory
{
    public interface ICreateIdentiferFactory
    {
        public BaseIdentiferKind CreateIdentifer(BaseIdentiferType identiferType, IKind parent);
    }
    
    public class DefaultCreateIdentiferFactory : ICreateIdentiferFactory
    {
        public BaseIdentiferKind CreateIdentifer(BaseIdentiferType identiferType, IKind parent)
        {
            switch (identiferType)
            {
                case ClassType:
                    return new ClassKind(identiferType, parent);
                case StructType:
                    return new StructKind(identiferType, parent);
            }

            throw new NotSupportedException(identiferType.GetType().Name);
        }
    }
}