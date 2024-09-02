using System;

namespace Attributes
{
    /// <summary>
    /// 协议
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ProtocolAttribute : Attribute
    {
        private readonly Node m_Nodes;
        private readonly int m_MaxSize;
        
        public ProtocolAttribute(Node nodes, int maxSize = -1)
        {
            m_Nodes = nodes;
            m_MaxSize = maxSize;
        }
    }
}
