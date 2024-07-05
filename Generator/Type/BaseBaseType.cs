using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator;

/// <summary>
/// 基本类型(string算作基本类型)
/// </summary>
public abstract class BaseBaseType : IType
{
    public IType Parse(TypeSyntax typeSyntax)
    {
        return this;
    }

    public void Compile()
    {
        
    }
}