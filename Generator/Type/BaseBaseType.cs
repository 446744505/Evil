using Generator.Context;
using Generator.Visitor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Type
{
    /// <summary>
    /// 基本类型(string算作基本类型)
    /// </summary>
    public abstract class BaseBaseType : IType
    {
        public IType Parse(TypeSyntax typeSyntax)
        {
            return this;
        }

        public IType Compile(CompileContext ctx)
        {
            return this;
        }
    
        public abstract void Accept<T>(ITypeVisitor<T> visitor) where T : ITypeVisitorContext;
    }
}