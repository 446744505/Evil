using Generator.Context;
using Generator.Kind;

namespace Generator.Factory
{
    public class XTableCreateFieldFactory : ICreateFieldFactory<XTableFieldKind>
    {
        public XTableFieldKind CreateField(NewFieldContext ctx, IKind parent)
        {
            return new XTableFieldKind(ctx.Name, ctx.Type, parent)
            {
                Comment = ctx.Comment
            };
        }
    }
}