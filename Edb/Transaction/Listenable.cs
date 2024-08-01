namespace Edb
{
    internal abstract partial class Listenable
    {
        internal static readonly Listenable DefaultListenable = new ListenableChanged("");

        public static Listenable Create(object obj)
        {
            if (obj is XBean xBean)
            {
                var lb = new ListenableBean();
                foreach (var f in XBeanInfo.GetFields(xBean))
                {
                    var ftName = f.FieldType.Name;
                    if (ftName.StartsWith("IDictionary"))
                        lb.Add(f.Name, new ListenableMap(f.Name));
                    else if (ftName.StartsWith("ISet"))
                        lb.Add(f.Name, new ListenableSet(f.Name));
                    else
                        lb.Add(f.Name, new ListenableChanged(f.Name));
                }

                return lb;
            }
            else
            {
                return DefaultListenable;
            }
        }

        internal abstract Task LogNotify<TKey, TValue>(object key, object value, RecordState rs, ListenerMap listenerMap)
            where TKey : notnull where TValue : class;
        internal abstract Listenable Copy();
        internal abstract void SetChanged<TKey, TValue>(LogNotify ln) 
            where TKey : notnull where TValue : class;

        private class ListenableChanged : Listenable
        {
            private readonly string m_VarName;
            private bool m_Changed;

            public ListenableChanged(string varName)
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
                        if (m_Changed)
                            await listenerMap.NotifyChanged(m_VarName, key, value, null);
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
    }
}