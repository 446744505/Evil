using System;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XBeanAttribute : Attribute
    {
        private readonly Node m_Nodes;
        
        public XBeanAttribute(Node nodes)
        {
            m_Nodes = nodes;
        }
    }
}