using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class FileContext
    {
        #region 字段

        private readonly GloableContext m_Gc;
        private readonly string m_Path;

        private readonly List<NamespaceDeclarationSyntax> m_Namespaces = new();
        
        #endregion

        public FileContext(GloableContext gc, string path)
        {
            m_Gc = gc;
            m_Path = path;
        }
        
        public void AddNamespace(NamespaceDeclarationSyntax ns)
        {
            m_Namespaces.Add(ns);
        }

        public void CreateFile()
        {
            if (m_Namespaces.Count == 0)
            {
                return;
            }
            var genFilePath = m_Path.Replace(".cs", Files.GeneratorFileSuffix);
            var outputPath = Path.Combine(m_Gc.OutPath, genFilePath);
            var dir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dir))
            {
                if (dir != null) Directory.CreateDirectory(dir);
            }
            
            var cu = SyntaxFactory.CompilationUnit();
            foreach (var ns in m_Namespaces)
            {
                cu = cu.AddUsings(ns.Usings.ToArray())
                    .AddMembers(ns);
            }
            var code = cu.NormalizeWhitespace().ToFullString();
            File.WriteAllText(outputPath, code);
        }
    }
}