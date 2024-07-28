using System;

namespace Attributes
{
    public class XTableAttribute : Attribute
    {
        private Node m_Nodes;
        private string m_Lock;
        private int m_Capacity;
        
        public XTableAttribute(Node nodes, int capacity, Type? lockType = null)
        {
            m_Nodes = nodes;
            m_Capacity = capacity;
            m_Lock = lockType?.Name ?? string.Empty;
        }
    }
}