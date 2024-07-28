namespace Edb
{
    internal abstract class Listenable
    {
        internal static readonly Listenable DefaultListenable = new ListenableChanged("");

        public static Listenable Create(object obj)
        {
            if (obj is XBean xBean)
            {
                var lb = new ListenableBean();
                foreach (var f in XBeanInfo.GetFields(xBean))
                {
                    throw new NotImplementedException();
                }

                return lb;
            }
            else
            {
                return DefaultListenable;
            }
        }

        internal abstract void LogNotify<TKey, TValue>(object key, object value, RecordState rs, ListenerMap listenerMap)
            where TKey : notnull where TValue : class;
        internal abstract Listenable Copy();
        internal abstract void SetChanged<TKey, TValue>(LogNotify ln) 
            where TKey : notnull where TValue : class;

        private class ListenableBean : Listenable
        {
            private readonly Dictionary<string, Listenable> m_Vars = new();
            private bool m_Changed;
            
            public void Add(string varName, Listenable listenable)
            {
                m_Vars[varName] = listenable;
            }
            
            internal override void LogNotify<TKey, TValue>(object key, object value, RecordState rs, ListenerMap listenerMap)
            {
                foreach (var l in m_Vars.Values)
                    l.LogNotify<TKey, TValue>(key, value, rs, listenerMap);

                switch (rs)
                {
                    case RecordState.Added:
                        listenerMap.NotifyChanged("", key, value);
                        break;
                    case RecordState.Removed:
                        listenerMap.NotifyRemoved("", key, value);
                        break;
                    case RecordState.Changed:
                        if (m_Changed)
                            listenerMap.NotifyChanged("", key, value, null); 
                        break;
                }

                m_Changed = false;
            }

            internal override Listenable Copy()
            {
                var l = new ListenableBean();
                foreach (var (key, value) in m_Vars)
                {
                    l.Add(key, value.Copy());
                }

                return l;
            }

            internal override void SetChanged<TKey, TValue>(LogNotify ln)
            {
                m_Changed = true;
                m_Vars[ln.Pop().VarName].SetChanged<TKey, TValue>(ln);
            }
        }
        
        private class ListenableChanged : Listenable
        {
            private readonly string m_VarName;
            private bool m_Changed;

            public ListenableChanged(string varName)
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
                        if (m_Changed)
                            listenerMap.NotifyChanged(m_VarName, key, value, null);
                        break;
                }
                m_Changed = false;
            }

            internal override Listenable Copy()
            {
                return new ListenableChanged(m_VarName);
            }

            internal override void SetChanged<TKey, TValue>(LogNotify ln)
            {
                m_Changed = true;
            }
        }
        
        private class ListenableMap : Listenable
        {
            private readonly string m_VarName;
            private INote? m_Note;
            private List<XBean>? m_Changed;
            
            public ListenableMap(string varName)
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
                        if ((m_Note != null || m_Changed != null) && listenerMap.HasListener(m_VarName))
                        {
                            var nMap = m_Note == null ? new NoteMap<TKey, XBean>() : (NoteMap<TKey, XBean>)m_Note;
                            nMap.SetChanged(m_Changed!, XBeanInfo.GetValue((XBean)value, m_VarName)!);
                            listenerMap.NotifyChanged(m_VarName, key, value, nMap);
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