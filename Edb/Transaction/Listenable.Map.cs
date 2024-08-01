namespace Edb
{
    internal abstract partial class Listenable
    {
    private class ListenableMap : Listenable
        {
            private readonly string m_VarName;
            private INote? m_Note;
            private List<XBean>? m_Changed;
            
            public ListenableMap(string varName)
            {
                m_VarName = varName;
            }
            
            internal override async Task LogNotify<TKey, TValue>(object key, object value, RecordState rs, ListenerMap listenerMap)
            {
                switch (rs)
                {
                    case RecordState.Added:
                        await listenerMap.NotifyChanged(m_VarName, key, value);
                        break;
                    case RecordState.Removed:
                        await listenerMap.NotifyRemoved(m_VarName, key, value);
                        break;
                    case RecordState.Changed:
                        if ((m_Note != null || m_Changed != null) && listenerMap.HasListener(m_VarName))
                        {
                            var nMap = m_Note == null ? new NoteMap<TKey, XBean>() : (INoteMap)m_Note;
                            nMap.SetChanged(m_Changed!, XBeanInfo.GetValue((XBean)value, m_VarName)!);
                            await listenerMap.NotifyChanged(m_VarName, key, value, nMap);
                        }
                        break;
                }
                m_Note = null;
                m_Changed = null;
            }

            internal override Listenable Copy()
            {
                return new ListenableMap(m_VarName);
            }

            internal override void SetChanged<TKey, TValue>(LogNotify ln)
            {
                if (ln.IsLast)
                {
                    if (m_Note == null)
                        m_Note = ln.Note;
                    else
                        ((NoteMap<TKey, TValue>)m_Note).Merge(ln.Note);
                }
                else
                {
                    if (m_Changed == null)
                        m_Changed = new List<XBean>();
                    m_Changed.Add(ln.Pop().XBean);
                }
            }
        }
    }
}