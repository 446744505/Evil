using System.Collections.Generic;
using System.IO;
using System.Linq;
using Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;

namespace Generator.Context
{
    public class FileContext
    {
        #region 字段

        private readonly GloableContext m_Gc;
        private readonly string m_Path;
        private readonly CompilationUnitSyntax m_Root;
        
        private readonly List<NamespaceDeclarationSyntax> m_NamespaceSyntaxes = new();
        private readonly Dictionary<string, Kind.NamespaceKind> m_NamespaceKinds = new();

        #endregion

        #region 属性

        public GloableContext GloableContext => m_Gc;
        public Document Document { get; }
        public List<TypeContext> TypeContexts { get; } = new();
        public List<Kind.NamespaceKind> NamespaceKinds => m_NamespaceKinds.Values.ToList();

        #endregion

        public FileContext(GloableContext gc, CompilationUnitSyntax root, Document document, string path)
        {
            m_Gc = gc;
            m_Root = root;
            Document = document;
            m_Path = path;
            gc.AddFileContext(this);
        }

        public void AddNamespaceSyntax(NamespaceDeclarationSyntax ns)
        {
            m_NamespaceSyntaxes.Add(ns);
        }
        
        public void AddTypeContext(TypeContext tc)
        {
            TypeContexts.Add(tc);
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
                cu = cu.AddUsings(AnalysisUtil.SkipAttributes(m_Root.Usings).ToArray())
                    .AddMembers(ns);
            }
            var code = cu.NormalizeWhitespace().ToFullString();
            
            // 格式化代码
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();
            var workspace = MSBuildWorkspace.Create();
            var options = workspace.Options
                .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, false)
                .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 4)
                .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4)
                .WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, Files.NewLine);
            var formattedRoot = Formatter.Format(root, workspace, options);
            code = formattedRoot.ToFullString();
            
            File.WriteAllText(outputPath, code);
            m_Gc.Log($"生成文件:{outputPath}");
        }
    }
}