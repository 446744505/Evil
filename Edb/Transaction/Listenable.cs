namespace Edb
{
    internal abstract class Listenable
    {
        internal static readonly Listenable DefaultListenable = new ListenableChanged("");

        public static Listenable Create(object obj)
        {
            if (obj is XBean xBean)
            {
                return null;
            }
            else
            {
                return DefaultListenable;
            }
        }

        internal abstract void LogNotify(object key, object value, RecordState rs, ListenerMap listenerMap);
        internal abstract Listenable Copy();
        internal abstract void SetChanged(LogNotify ln);
        
        private class ListenableChanged : Listenable
        {
            private readonly string m_VarName;
            private bool m_Changed;

            public ListenableChanged(string varName)
            {
                m_VarName = varName;
            }

            internal override void LogNotify(object key, object value, RecordState rs, ListenerMap listenerMap)
            {
                
            }

            internal override Listenable Copy()
            {
                return new ListenableChanged(m_VarName);
            }

            internal override void SetChanged(LogNotify ln)
            {
                m_Changed = true;
            }
        }
    }
}