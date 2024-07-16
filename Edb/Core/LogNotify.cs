using System.Text;

namespace Edb
{
    internal sealed class LogNotify
    {
        private readonly INote m_Note;
        private readonly Deque<LogKey> m_Path = new();
        
        internal INote Note => m_Note;
        internal bool IsLast => m_Path.IsEmpty;

        private LogNotify(LogKey logKey, INote note)
        {
            m_Note = note;
            m_Path.AddBack(logKey);
        }

        internal LogKey Pop()
        {
            return m_Path.RemoveFront();
        }
        
        internal LogNotify Push(LogKey logKey)
        {
            m_Path.AddFront(logKey);
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