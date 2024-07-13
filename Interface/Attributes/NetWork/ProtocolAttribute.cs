using System;

namespace Attributes
{
    /// <summary>
    /// 协议
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ProtocolAttribute : Attribute
    {
        public Node m_Nodes;
        
        public ProtocolAttribute(Node nodes)
        {
            m_Nodes = nodes;
        }
    }

    [Flags]
    public enum Node
    {
        Client = 1,
        Game = 2,
        Map = 4,
    }
}
