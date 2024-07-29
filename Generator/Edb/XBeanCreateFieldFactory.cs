using Generator.Context;
using Generator.Kind;

namespace Generator.Factory
{
    public class XBeanCreateFieldFactory : ICreateFieldFactory<XBeanFieldKind>
    {
        public XBeanFieldKind CreateField(NewFieldContext ctx, IKind parent)
        {
            return new XBeanFieldKind(ctx.Name, ctx.Type, parent)
            {
                Comment = ctx.Comment
            };
        }
    }
}