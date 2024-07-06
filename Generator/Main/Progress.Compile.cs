namespace Generator
{
    public partial class Progress
    {
        public void Compile()
        {
            CompileContext context = new();
            context.IdentiferFind = name =>
            {
                return m_Gc.FindIdentiferKind(name);
            };
            foreach (var fc in m_Gc.FileContexts)
            {
                foreach (var namespaceKind in fc.NamespaceKinds)
                {
                    foreach (var identiferKind in namespaceKind.Children())
                    {
                        identiferKind.Compile(context);
                    }
                }
            }
        }
    }
}