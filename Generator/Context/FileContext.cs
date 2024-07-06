using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Context
{
    public class FileContext
    {
        #region 字段

        private readonly GloableContext m_Gc;
        private readonly string m_Path;
        private readonly Document m_Document;

        private readonly List<TypeContext> m_TypeContexts = new();
        private readonly List<NamespaceDeclarationSyntax> m_NamespaceSyntaxes = new();
        private readonly Dictionary<string, Kind.NamespaceKind> m_NamespaceKinds = new();

        #endregion

        #region 属性

        public Document Document => m_Document;
        public List<TypeContext> TypeContexts => m_TypeContexts;
        public List<Kind.NamespaceKind> NamespaceKinds => m_NamespaceKinds.Values.ToList();

        #endregion

        public FileContext(GloableContext gc, Document document, string path)
        {
            m_Gc = gc;
            m_Document = document;
            m_Path = path;
            gc.AddFileContext(this);
        }
        
        public void AddNamespaceSyntax(NamespaceDeclarationSyntax ns)
        {
            m_NamespaceSyntaxes.Add(ns);
        }
        
        public void AddTypeContext(TypeContext tc)
        {
            m_TypeContexts.Add(tc);
        }
        
        public Kind.NamespaceKind GetOrCreateNamespaceKind(string name)
        {
            if (m_NamespaceKinds.TryGetValue(name, out var ns))
            {
                return ns;
            }
            ns = new Kind.NamespaceKind(name);
            m_NamespaceKinds.Add(name, ns);
            return ns;
        }

        public void CreateFile()
        {
            if (m_NamespaceSyntaxes.Count == 0)
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
            foreach (var ns in m_NamespaceSyntaxes)
            {
                cu = cu.AddUsings(ns.Usings.ToArray())
                    .AddMembers(ns);
            }
            var code = cu.NormalizeWhitespace().ToFullString();
            File.WriteAllText(outputPath, code);
            m_Gc.Log($"生成文件:{outputPath}");
        }
    }
}