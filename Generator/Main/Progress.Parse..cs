using Generator.AttributeHandler;
using Generator.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public partial class Progress
    {
        private static readonly List<string> SkipDirs = new()
        {
            "Attributes",
        };
        
        private readonly GloableContext m_Gc;

        public Progress(GloableContext gc)
        {
            m_Gc = gc;
        }

        public FileContext? ParseDocument(Document document)
        {
            var path = document.FilePath!;
            if (IsSkipFile(path))
            {
                return null;
            }
            
            return ParseDocument0(document);
        }
        
        /// <summary>
        /// 语法解析单个文件
        /// </summary>
        /// <param name="document">源文件</param>
        /// <param name="gc">全局上下文</param>
        /// <exception cref="NullReferenceException"></exception>
        private FileContext ParseDocument0(Document document)
        {
            var filePath = document.FilePath!;
            // 获取相对路径
            var relativePath = Path.GetRelativePath(Path.GetDirectoryName(CmdLine.I.InterfaceProject)!, filePath);
            // 创建文件上下文
            var fc = new FileContext(m_Gc, document, relativePath);
            // 解析语法树
            var tree = document.GetSyntaxTreeAsync().Result!;
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

            return fc;
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