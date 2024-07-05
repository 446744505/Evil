namespace Generator
{
    public abstract class BaseKind : IKind
    {
        private readonly IKind m_Parent;
        private readonly List<IKind> m_Children = new();

        protected BaseKind(IKind parent)
        {
            m_Parent = parent;
            parent?.AddChild(this);
        }

        public IKind Parent()
        {
            return m_Parent;
        }

        public void AddChild(IKind child)
        {
            m_Children.Add(child);
        }
    }
}