using System;

namespace Attributes
{
    /// <summary>
    /// 添加到xtable的字段上
    /// 会生成该字段的xlistener和eevent
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class XListenerAttribute : Attribute
    {
    }
}