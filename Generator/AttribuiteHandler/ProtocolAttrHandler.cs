using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class ProtocolAttrHandler : BaseTypeAttrHandler
    {
        protected override void Parse0(TypeContext tc, AttributeSyntax attr)
        {
            
        }

        public override string GetAttrName()
        {
            return Attributes.Protocol;
        }
    }
}