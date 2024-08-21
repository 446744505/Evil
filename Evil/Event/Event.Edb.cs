namespace Evil.Event;

/// <summary>
/// edb的listener抛出的事件
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class EEvent<TKey> : IEvent
{
    private readonly TKey m_Key;

    protected EEvent(TKey key)
    {
        m_Key = key;
    }

    /// <summary>
    /// changed table key
    /// </summary>
    public TKey Key => m_Key;
}