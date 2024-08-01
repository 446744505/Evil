namespace Edb
{
    internal abstract partial class Listenable
    {
        private class ListenableBean : Listenable
        {
            private readonly Dictionary<string, Listenable> m_Vars = new();
            private bool m_Changed;
            
            public void Add(string varName, Listenable listenable)
            {
                m_Vars[varName] = listenable;
            }
            
            internal override async Task LogNotify<TKey, TValue>(object key, object value, RecordState rs, ListenerMap listenerMap)
            {
                foreach (var l in m_Vars.Values)
                    await l.LogNotify<TKey, TValue>(key, value, rs, listenerMap);

                switch (rs)
                {
                    case RecordState.Added:
                        await listenerMap.NotifyChanged("", key, value);
                        break;
                    case RecordState.Removed:
                        await listenerMap.NotifyRemoved("", key, value);
                        break;
                    case RecordState.Changed:
                        if (m_Changed)
                            await listenerMap.NotifyChanged("", key, value, null); 
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
    }
}