using System;
using System.Collections.Generic;
using System.Text;
using Generator.Exception;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Util
{
    public class AnalysisUtil
    {
        public static NamespaceDeclarationSyntax GetNameSpaceSyntax(TypeDeclarationSyntax type)
        {
            foreach (var syntaxNode in type.AncestorsAndSelf())
            {
                if (syntaxNode is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    return namespaceDeclaration;
                }
            }

            throw new AnalysException($"{type.Identifier}没有找到命名空间");
        }
        
        public static NamespaceDeclarationSyntax GetNameSpaceSyntax(IdentifierNameSyntax type)
        {
            foreach (var syntaxNode in type.AncestorsAndSelf())
            {
                if (syntaxNode is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    return namespaceDeclaration;
                }
            }

            throw new AnalysException($"{type.Identifier}没有找到命名空间");
        }

        public static string GetClassName(TypeDeclarationSyntax type)
        {
            return type.Identifier.Text;
        }
        
        public static string GetFieldName(FieldDeclarationSyntax field)
        {
            return field.Declaration.Variables.First().Identifier.Text;
        }

        private static string GetLeadingComment(SyntaxTriviaList triviaList)
        {
            foreach (var trivia in triviaList)
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    // 解析出summary部分内容
                    var triviaText = trivia.ToString();
                    var index = triviaText.IndexOf("<summary>", StringComparison.Ordinal);
                    if (index != -1)
                    {
                        var endIndex = triviaText.IndexOf("</summary>", StringComparison.Ordinal);
                        if (endIndex != -1)
                        {
                            var str= triviaText.Substring(index + 9, endIndex - index - 9);
                            var lines = str.Split("///");
                            var sb = new StringBuilder();
                            foreach (var line in lines)
                            {
                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    continue;
                                }
                                sb.Append(line.Trim().TrimStart());
                            }
                            
                            return sb.ToString();
                        }
                    }
                }
            }

            return string.Empty;
        }

        private static string GetTrailingComment(SyntaxTriviaList triviaList)
        {
            var comment = triviaList.ToString();
            // 删除注释符号
            comment = comment.Replace("//", string.Empty);
            // 删除换行
            comment = comment.Replace("\n", string.Empty);
            comment = comment.Replace("\r", string.Empty);
            // 删除前后面的空格
            comment = comment.Trim().TrimStart();
            if (!string.IsNullOrWhiteSpace(comment))
            {
                return comment;
            }

            return string.Empty;
        }

        public static string GetComment(TypeDeclarationSyntax type)
        {
            var trailingComment = GetTrailingComment(type.GetTrailingTrivia());
            if (!string.IsNullOrWhiteSpace(trailingComment))
            {
                return trailingComment;
            }
            return GetLeadingComment(type.GetLeadingTrivia());
        }
        
        public static string GetComment(MethodDeclarationSyntax type)
        {
            var trailingComment = GetTrailingComment(type.GetTrailingTrivia());
            if (!string.IsNullOrWhiteSpace(trailingComment))
            {
                return trailingComment;
            }
            return GetLeadingComment(type.GetLeadingTrivia());
        }
        
        public static string GetComment(FieldDeclarationSyntax field)
        {
            var trailingComment = GetTrailingComment(field.GetTrailingTrivia());
            if (!string.IsNullOrWhiteSpace(trailingComment))
            {
                return trailingComment;
            }
            return GetLeadingComment(field.GetLeadingTrivia());
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
        
        public static SyntaxList<UsingDirectiveSyntax> SkipAttributes(SyntaxList<UsingDirectiveSyntax> usings)
        {
            var list = new SyntaxList<UsingDirectiveSyntax>();
            foreach (var syntax in usings)
            {
                if (syntax.Name?.ToString() == Namespaces.AttributesNamespace)
                {
                    continue;
                }
                list = list.Add(syntax);
            }

            return list;
        }
    }
}