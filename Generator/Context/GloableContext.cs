namespace Generator
{
    public class GloableContext
    {
        #region 字段

        private readonly List<FileContext> m_FileContexts = new();

        #endregion
        #region 属性

        public string OutPath { get; init; }

        #endregion
        public GloableContext(string outPath)
        {
            OutPath = outPath;
        }
        
        public void AddFileContext(FileContext fc)
        {
            m_FileContexts.Add(fc);
        }
        
        public void CleanGeneratedFiles()
        {
            foreach (var file in Directory.GetFiles(OutPath, $"*{Files.GeneratorFileSuffix}", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }
        }
    }
}