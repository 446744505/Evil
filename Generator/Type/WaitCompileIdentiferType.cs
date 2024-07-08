using System;
using Generator.Context;
using Generator.Visitor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Type
{
    public class WaitCompileIdentiferType : IType
    {
        public string Name { get; }
    
        public WaitCompileIdentiferType(string name)
        {
            Name = name;
        }
    
        public IType Parse(TypeSyntax typeSyntax)
        {
            return this;
        }

        public IType Compile(CompileContext ctx)
        {
            var kind = ctx.IdentiferFind.Invoke(Name);
            return kind.CreateIdentiferType();
        }

        public void Accept(ITypeVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}