using System;

namespace Attributes;

/// <summary>
/// 协议字段
/// </summary>
[AttributeUsage(AttributeTargets.Field|AttributeTargets.Parameter)]
public class ProtocolFieldAttribute : Attribute
{
    private readonly int m_Index;
    
    public ProtocolFieldAttribute(int index)
    {
        m_Index = index;
    }
}