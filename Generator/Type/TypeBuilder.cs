using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class TypeBuilder
    {
        public static readonly TypeBuilder I = new();
        
        private TypeBuilder() { }

        public ClassType ParseType(TypeDeclarationSyntax typeSyntax)
        {
            var kind = typeSyntax.Kind();
            return kind switch
            {
                SyntaxKind.ClassDeclaration => new ClassType().Parse(typeSyntax),
                _ => throw new TypeException($"不支持的类型:{kind.ToString()}")
            };
        }

        public IType ParseType(TypeSyntax typeSyntax)
        {
            var kind = typeSyntax.Kind();
            return kind switch
            {
                SyntaxKind.PredefinedType => ParsePredefinedType((typeSyntax as PredefinedTypeSyntax)!),
                SyntaxKind.GenericName => ParseGenericType((typeSyntax as GenericNameSyntax)!),
                SyntaxKind.IdentifierName => ParseIdentiferType((typeSyntax as IdentifierNameSyntax)!),
                _ => throw new TypeException($"不支持的类型:{kind.ToString()}")
            };
        }

        private IType ParseGenericType(GenericNameSyntax typeSyntax)
        {
            return typeSyntax.Identifier.Text switch
            {
                "List" => new ListType().Parse(typeSyntax),
                "Dictionary" => new MapType().Parse(typeSyntax),
                _ => throw new TypeException($"不支持的泛型类型:{typeSyntax.Identifier.Text}")
            };
        }

        private IType ParsePredefinedType(PredefinedTypeSyntax typeSyntax)
        {
            var kind = typeSyntax.Keyword.Kind();
            return kind switch
            {
                SyntaxKind.IntKeyword => new IntType(),
                SyntaxKind.LongKeyword => new LongType(),
                SyntaxKind.StringKeyword => new StringType(),
                SyntaxKind.BoolKeyword => new BoolType(),
                SyntaxKind.FloatKeyword => new FloatType(),
                SyntaxKind.DoubleKeyword => new DoubleType(),
                _ => throw new TypeException($"不支持的类型:{kind.ToString()}")
            };
        }
        private IType ParseIdentiferType(IdentifierNameSyntax typeSyntax)
        {
            return new ClassType().Parse(typeSyntax);
        }
    }
}