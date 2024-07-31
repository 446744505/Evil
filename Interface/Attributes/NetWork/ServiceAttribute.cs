using System;

namespace Attributes
{
    /// <summary>
    /// 客户端到服务器的请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute
    {
        private readonly Node m_ClientNode;
        private readonly Node m_ServerNode;

        public ServiceAttribute(Node clientNode, Node serverNode)
        {
            m_ClientNode = clientNode;
            m_ServerNode = serverNode;
        }
    }
}