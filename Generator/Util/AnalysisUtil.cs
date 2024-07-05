using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Util
{
    public class AnalysisUtil
    {
        public static string GetNameSpaceName(TypeDeclarationSyntax type)
        {
            foreach (var syntaxNode in type.AncestorsAndSelf())
            {
                if (syntaxNode is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    return namespaceDeclaration.Name.ToString();
                }
            }

            return string.Empty;
        }

        public static string GetClassName(TypeDeclarationSyntax type)
        {
            return type.Identifier.Text;
        }
        
        public static string GetFieldName(FieldDeclarationSyntax field)
        {
            return field.Declaration.Variables.First().Identifier.Text;
        }

        public static bool HadAttribute(MemberDeclarationSyntax type, string attrName, out AttributeSyntax? syntax)
        {
            foreach (var attr in type.AttributeLists)
            {
                foreach (var attrNode in attr.Attributes)
                {
                    if (attrNode.Name.ToString() == attrName)
                    {
                        syntax = attrNode;
                        return true;
                    }
                }
            }

            syntax = null;
            return false;
        }

        public static bool HadAttribute(BaseParameterSyntax type, string attrName, out AttributeSyntax? syntax)
        {
            foreach (var attr in type.AttributeLists)
            {
                foreach (var attrNode in attr.Attributes)
                {
                    if (attrNode.Name.ToString() == attrName)
                    {
                        syntax = attrNode;
                        return true;
                    }
                }
            }

            syntax = null;
            return false;
        }

        public static bool HadAttrArgument(AttributeSyntax attr, int index, out string val)
        {
            var argList = attr.ArgumentList;
            if (argList == null || argList.Arguments.Count <= index)
            {
                val = string.Empty;
                return false;
            }
            val = argList.Arguments[index].Expression.ToString();
            return true;
        }
    }
}