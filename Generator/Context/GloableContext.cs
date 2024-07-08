using System;
using System.Collections.Generic;
using System.IO;

namespace Generator.Context
{
    public partial class GloableContext
    {
        #region 属性

        public string OutPath { get; }
        public List<FileContext> FileContexts { get; } = new();

        #endregion
        public GloableContext(string outPath)
        {
            OutPath = outPath;
        }
        
        public void AddFileContext(FileContext fc)
        {
            FileContexts.Add(fc);
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