namespace Generator
{
    public class GloableContext
    {
        #region 属性

        public string OutPath { get; init; }

        #endregion
        public GloableContext(string outPath)
        {
            OutPath = outPath;
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