namespace Attributes;

/// <summary>
/// 协议字段
/// </summary>
[AttributeUsage(AttributeTargets.Field|AttributeTargets.Parameter)]
public class ProtocolFieldAttribute : Attribute
{
    private int _index;
    
    public ProtocolFieldAttribute(int index)
    {
        _index = index;
    }
}