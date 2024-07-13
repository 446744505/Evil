using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Generator.Context
{
    public partial class GloableContext
    {
        #region 属性

        public bool IsGame => CmdLine.I.Node == Nodes.Server;
        public bool IsClient => CmdLine.I.Node == Nodes.Client;

        public string OutPath { get; }
        public Project Project { get;}
        public Compilation Compilation { get; }
        public List<FileContext> FileContexts { get; } = new();
        /// <summary>
        /// proto里是message(req、ntf、ack)的class name
        /// </summary>
        public HashSet<string> ProtocolMessageNames { get; } = new();

        #endregion
        public GloableContext(Project project, string outPath)
        {
            Project = project;
            OutPath = outPath;
            Compilation = project.GetCompilationAsync().Result!;
        }
        
        public void AddFileContext(FileContext fc)
        {
            FileContexts.Add(fc);
        }
        
        public void AddProtocolMessageName(string name)
        {
            ProtocolMessageNames.Add(name);
        }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return Compilation.GetSemanticModel(tree);
        }

        public bool IsNodeAt(string nodes)
        {
            if (string.IsNullOrEmpty(nodes))
            {
                return false;
            }
            // 根据Interface里的Node枚举名字判断
            // 特性解析后的nodes格式为: "Node.Client|Node.Game"
            var nodeArr = nodes.Split('|');
            var parsedNodes = new List<string>();
            foreach (var node in nodeArr)
            {
                parsedNodes.Add(node.Split(".")[1].TrimStart().TrimEnd().ToLower());
            }
            return parsedNodes.Contains(CmdLine.I.Node);
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