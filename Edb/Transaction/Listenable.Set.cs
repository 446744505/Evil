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

            internal override async Task LogNotify<TKey, TValue>(object key, object value, RecordState rs, ListenerMap listenerMap, TransactionCtx ctx)
            {
                switch (rs)
                {
                    case RecordState.Added:
                        await listenerMap.NotifyChanged(m_VarName, key, value, ctx);
                        break;
                    case RecordState.Removed:
                        await listenerMap.NotifyRemoved(m_VarName, key, value, ctx);
                        break;
                    case RecordState.Changed:
                        if (m_Note != null)
                            await listenerMap.NotifyChanged(m_VarName, key, value, m_Note, ctx);
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