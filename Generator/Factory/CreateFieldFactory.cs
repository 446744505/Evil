using Generator.Context;
using Generator.Kind;

namespace Generator.Factory
{
    public interface ICreateFieldFactory<out T> where T : FieldKind
    {
        public T CreateField(NewFieldContext ctx, IKind parent);
    }
    
    public class DefaultCreateFieldFactory : ICreateFieldFactory<FieldKind>
    {
        public FieldKind CreateField(NewFieldContext ctx, IKind parent)
        {
            return new FieldKind(ctx.Name, ctx.Type, parent)
            {
                Comment = ctx.Comment,
            };
        }
    }
}