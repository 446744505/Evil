using Microsoft.CodeAnalysis.CSharp;

namespace Generator;

public static class Program
{
    public static void Main(string[] args)
    {
        var tree = CSharpSyntaxTree.ParseText("");
    }
}


