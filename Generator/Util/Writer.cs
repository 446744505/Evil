using System.Text;

namespace Generator.Util
{
    public class Writer
    {
        private readonly StringBuilder m_Sb = new();

        public void WriteLine()
        {
            m_Sb.Append(Files.NewLine);
        }

        public void WriteLine(string content)
        {
            m_Sb.Append(content);
            m_Sb.Append(Files.NewLine);
        }

        public void WriteLine(int tabCnt, string content)
        {
            for (var i = 0; i < tabCnt; i++)
            {
                m_Sb.Append(" ");
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