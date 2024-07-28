using System;
using Generator.Kind;

namespace Generator.Context
{
    public partial class GloableContext
    {
        public BaseIdentiferKind FindIdentiferKind<T>(string name) where T : NamespaceKind
        {
            foreach (var fc in FileContexts)
            {
                foreach (var namespaceKind in fc.FindNamespaceKinds<T>())
                {
                    foreach (var classKind in namespaceKind.Children())
                    {
                        if (classKind.Name == name)
                        {
                            return classKind;
                        }
                    }
                }
            }

            throw new NullReferenceException($"不存在的类型{name}");
        }
    }
}