using System;
using Generator.Context;
using Generator.Visitor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Type
{
    public class WaitCompileIdentiferType : IType
    {
        private readonly string m_Name;
        public string Name => m_Name;
    
        public WaitCompileIdentiferType(string name)
        {
            m_Name = name;
        }
    
        public IType Parse(TypeSyntax typeSyntax)
        {
            return this;
        }

        public IType Compile(CompileContext ctx)
        {
            var kind = ctx.IdentiferFind.Invoke(m_Name);
            return kind.CreateIdentiferType();
        }

        public void Accept<T>(ITypeVisitor<T> visitor) where T : ITypeVisitorContext
        {
            throw new NotImplementedException();
        }
    }
}