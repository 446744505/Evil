using Generator.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.AttributeHandler
{
    public class XBeanAttrHandler : BaseTypeAttrHandler
    {
        public XBeanAttrHandler(TypeContext typeContext, AttributeSyntax attr) : base(typeContext, attr)
        {
        }
        
        protected override void Parse0()
        {
            
        }
    }
}