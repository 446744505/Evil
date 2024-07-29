
using System.Text;

namespace Generator.Util
{
    public class Writer
    {
        private bool m_IsFirst = true;
        private bool m_IsMultiLine;
        private int m_NotFirstTabCnt;
        
        private readonly StringBuilder m_Sb = new();

        public Writer(bool isMultiLine = false, int notFirstTabCnt = 0)
        {
            m_IsMultiLine = isMultiLine;
            m_NotFirstTabCnt = notFirstTabCnt;
        }

        public void WriteLine()
        {
            m_Sb.Append(Files.NewLine);
        }

        public void WriteLine(string content)
        {
            if (m_IsMultiLine && !m_IsFirst)
            {
                WriteLine(m_NotFirstTabCnt, content);
            }
            else
            {
                m_Sb.Append(content);
                m_Sb.Append(Files.NewLine);
                m_IsFirst = false;
            }
        }

        public void WriteLine(int tabCnt, string content)
        {
            for (var i = 0; i < tabCnt; i++)
            {
                m_Sb.Append("    ");
            }

            m_Sb.Append(content);
            m_Sb.Append(Files.NewLine);
        }

        public override string ToString()
        {
            return m_Sb.ToString();
        }
    }
}