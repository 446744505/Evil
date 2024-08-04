using System;

namespace Attributes
{
    public class BindAttribute : Attribute
    {
        private readonly Node m_Nodes;

        public BindAttribute(Node nodes)
        {
            m_Nodes = nodes;
        }
    }
}