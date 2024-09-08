using System.Text;

namespace Edb
{
    internal sealed class LogNotify
    {
        private readonly INote m_Note;
        private readonly Nito.Collections.Deque<LogKey> m_Path = new();
        
        internal INote Note => m_Note;
        internal bool IsLast => m_Path.Count == 0;

        private LogNotify(LogKey logKey, INote note)
        {
            m_Note = note;
            m_Path.AddToBack(logKey);
        }

        internal LogKey Pop()
        {
            return m_Path.RemoveFromFront();
        }
        
        internal LogNotify Push(LogKey logKey)
        {
            m_Path.AddToFront(logKey);
            return this;
        }
        
        internal static void Notify(LogKey logKey, INote note)
        {
            logKey.XBean.Notify(new LogNotify(logKey, note));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var logKey in m_Path)
            {
                sb.Append('.').Append(logKey.VarName);
            }

            sb.Append('=').Append(m_Note);
            return sb.ToString();
        }
    }
}