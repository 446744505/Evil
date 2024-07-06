using Generator.Util;

namespace Generator.Context
{
    public class WriterContext
    {
        #region 字段

        private readonly string m_FilePath;
        private readonly Writer m_Writer = new();

        #endregion

        #region 属性

        public Writer Writer => m_Writer;

        #endregion

        public WriterContext(string filePath)
        {
            m_FilePath = filePath;
        }
    }
}