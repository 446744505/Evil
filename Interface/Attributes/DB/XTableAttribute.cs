using System;

namespace Attributes
{
    public class XTableAttribute : Attribute
    {
        private Node m_Nodes;
        private string m_Lock;
        private int m_Capacity;
        private bool m_IsMemory;
        
        public XTableAttribute(Node nodes, int capacity, Type? lockType = null, bool memory = false)
        {
            m_Nodes = nodes;
            m_Capacity = capacity;
            m_Lock = lockType?.Name ?? string.Empty;
            m_IsMemory = memory;
        }
    }
}