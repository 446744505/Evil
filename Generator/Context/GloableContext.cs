namespace Generator.Context
{
    public partial class GloableContext
    {
        #region 字段

        private readonly List<FileContext> m_FileContexts = new();

        #endregion
        #region 属性

        public string OutPath { get; }
        public List<FileContext> FileContexts => m_FileContexts;

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
        
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Exception(System.Exception e)
        {
            Console.WriteLine(e);
        }
    }
}