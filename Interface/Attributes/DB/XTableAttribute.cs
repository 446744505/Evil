using System;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XTableAttribute : Attribute
    {
        private readonly Node m_Nodes;
        private readonly string m_Lock;
        private readonly int m_Capacity;
        private readonly bool m_IsMemory;
        
        public XTableAttribute(Node nodes, int capacity, Type? lockType = null, bool memory = false)
        {
            m_Nodes = nodes;
            m_Capacity = capacity;
            m_Lock = lockType?.Name ?? string.Empty;
            m_IsMemory = memory;
        }
    }
}