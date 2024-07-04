using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator
{
    public class ProtocolFieldAttrHandler : IAttributeHandler
    {
        public void Parse(TypeContext tc, AttributeSyntax attr)
        {
        
        }

        public string GetAttrName()
        {
            return Attributes.ProtocolField;
        }
    }
}