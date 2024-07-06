using Generator.Context;
using Generator.Kind;

namespace Generator.Factory
{
    public class ProtoCreateFieldFactory : ICreateFieldFactory<ProtoFieldKind>
    {
        public ProtoFieldKind CreateField(NewFieldContext ctx, IKind parent)
        {
            return new ProtoFieldKind(ctx.Name, ctx.Type, parent)
            {
                Comment = ctx.Comment
            };
        }
    }
}