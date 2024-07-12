using System;
using System.Collections.Generic;
using System.IO;

namespace Generator.Context
{
    public partial class GloableContext
    {
        #region 属性

        public bool IsGame => CmdLine.I.Node == Nodes.Server;
        public bool IsClient => CmdLine.I.Node == Nodes.Client;

        public string OutPath { get; }
        public List<FileContext> FileContexts { get; } = new();
        /// <summary>
        /// proto里是message(req、ntf、ack)的class name
        /// </summary>
        public HashSet<string> ProtocolMessageNames { get; } = new();

        #endregion
        public GloableContext(string outPath)
        {
            OutPath = outPath;
        }
        
        public void AddFileContext(FileContext fc)
        {
            FileContexts.Add(fc);
        }
        
        public void AddProtocolMessageName(string name)
        {
            ProtocolMessageNames.Add(name);
        }
        
        public void CleanGeneratedFiles()
        {
            if (!Path.Exists(OutPath))
            {
                return;
            }
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