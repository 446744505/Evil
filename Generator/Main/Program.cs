using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator;

public static class Program
{
    public static void Main(string[] args)
    {
        CmdLine.Init(args);
        foreach (var sourceFile in SourceFiles())
        {  
            ParseFile(sourceFile);
        }
    }

    private static string[] SourceFiles()
    {
        var interfacePath = CmdLine.I.InterfacePath;
        return Directory.GetFiles(interfacePath, "*.cs", SearchOption.AllDirectories);
    }

    private static void ParseFile(string path)
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
        var root = tree.GetRoot();
        var types = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
        foreach (var type in types)
        {
            Console.WriteLine(type.ToString());
        }
    }
}


