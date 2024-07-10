using System.Collections.Generic;
using Generator.Context;

namespace Generator.Kind
{
    public abstract class BaseKind : IKind
    {
        private readonly IKind? m_Parent;
        private readonly List<IKind> m_Children = new();

        protected BaseKind(IKind? parent)
        {
            m_Parent = parent;
            parent?.AddChild(this);
        }

        public IKind Parent()
        {
            return m_Parent!;
        }

        public void AddChild(IKind child)
        {
            m_Children.Add(child);
        }

        public List<IKind> Children()
        {
            return m_Children;
        }

        public virtual void Compile(CompileContext ctx)
        {
            Compile0(ctx);
            foreach (var child in m_Children)
            {
                child.Compile(ctx);
            }
        }
        
        protected abstract void Compile0(CompileContext ctx);
    }
}