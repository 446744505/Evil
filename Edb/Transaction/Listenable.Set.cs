namespace Edb
{
    internal abstract partial class Listenable
    {
        private class ListenableSet : Listenable
        {
            private readonly string m_VarName;
            private INote? m_Note;

            public ListenableSet(string varName)
            {
                m_VarName = varName;
            }

            internal override void LogNotify<TKey, TValue>(object key, object value, RecordState rs, ListenerMap listenerMap)
            {
                switch (rs)
                {
                    case RecordState.Added:
                        listenerMap.NotifyChanged(m_VarName, key, value);
                        break;
                    case RecordState.Removed:
                        listenerMap.NotifyRemoved(m_VarName, key, value);
                        break;
                    case RecordState.Changed:
                        if (m_Note != null)
                            listenerMap.NotifyChanged(m_VarName, key, value, m_Note);
                        break;
                }
                m_Note = null;
            }

            internal override Listenable Copy()
            {
                return new ListenableSet(m_VarName);
            }

            internal override void SetChanged<TKey, TValue>(LogNotify ln)
            {
                if (!ln.IsLast)
                    return;
                if (m_Note == null)
                    m_Note = ln.Note;
                else
                    ((NoteSet<TValue>)m_Note).Merge(ln.Note);
            }
        }
    }
}