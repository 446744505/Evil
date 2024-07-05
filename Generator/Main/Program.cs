
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public static class Program
    {
        private static readonly List<string> SkipDirs = new List<string>
        {
            "Attributes",
        };
        
        public static void Main(string[] args)
        {
            // 解析命令行参数
            CmdLine.Init(args);
            // 初始化全局上下文
            var gc = new GloableContext(CmdLine.I.CodeOutputPath);
            // 删除旧的生成文件
            gc.CleanGeneratedFiles();
            // 遍历所有源文件并处理
            foreach (var sourceFile in SourceFiles())
            {
                if (IsSkipFile(sourceFile))
                {
                    continue;
                }

                try
                {
                    HandleFile(sourceFile, gc);   
                } catch (Exception e)
                {
                    throw new Exception($"解析文件{sourceFile}失败", e);
                }
            }
        }

        /// <summary>
        /// 根据命令行参数获取所有源文件
        /// </summary>
        /// <returns></returns>
        private static string[] SourceFiles()
        {
            var interfacePath = CmdLine.I.InterfacePath;
            return Directory.GetFiles(interfacePath, $"*{Files.CodeFileSuffix}", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 语法解析单个文件
        /// </summary>
        /// <param name="filePath">源文件路径</param>
        /// <param name="gc">全局上下文</param>
        /// <exception cref="NullReferenceException"></exception>
        private static void HandleFile(string filePath, GloableContext gc)
        {
            // 获取相对路径
            var relativePath = Path.GetRelativePath(CmdLine.I.InterfacePath, filePath);
            // 创建文件上下文
            var fc = new FileContext(gc, relativePath);
            // 解析语法树
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
            // 获取该文件下所有类型：class、struct、interface
            var types = tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach (var type in types)
            {
                // 创建类型上下文
                var tc = new TypeContext(fc, type);
                // 获取所有特性
                var attributes = type.AttributeLists.SelectMany(x => x.Attributes);
                foreach (var attribute in attributes)
                {
                    var attributeName = attribute.Name.ToString();
                    var handler = AttrHandlerMgr.I.CreateHandler(attributeName);
                    // 找到特效处理器，解析特性
                    handler.Parse(tc, attribute);
                }
            }

            fc.CreateFile();
        }

        private static bool IsSkipFile(string path)
        {
            // path所在的目录里是否包含SkipDirs中的任意一个
            var dir = Path.GetDirectoryName(path);
            // 将目录路径拆分为各个部分
            var directories = dir!.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return directories.Any(d => SkipDirs.Contains(d));
        }
    }
}


