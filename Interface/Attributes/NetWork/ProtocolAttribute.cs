using System;

namespace Attributes
{
    /// <summary>
    /// 协议
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ProtocolAttribute : Attribute
    {
        private Node m_Nodes;
        
        public ProtocolAttribute(Node nodes)
        {
            m_Nodes = nodes;
        }
    }
}
