using System;

namespace Attributes
{
    public class XBeanAttribute : Attribute
    {
        private Node m_Nodes;
        
        public XBeanAttribute(Node nodes)
        {
            m_Nodes = nodes;
        }
    }
}