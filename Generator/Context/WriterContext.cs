using Generator.Util;

namespace Generator.Context
{
    public class WriterContext
    {
        #region 字段

        private readonly string m_FilePath;

        #endregion

        #region 属性

        public Writer Writer { get; } = new();

        #endregion

        public WriterContext(string filePath)
        {
            m_FilePath = filePath;
        }
    }
}