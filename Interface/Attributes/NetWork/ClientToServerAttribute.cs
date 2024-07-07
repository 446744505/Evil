using System;

namespace Attributes
{
    /// <summary>
    /// 客户端到服务器的请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ClientToServerAttribute : Attribute
    {

    }
}