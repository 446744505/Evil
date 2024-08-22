using Generator.Context;
using Generator.Exception;
using Generator.Type;

namespace Generator.Kind
{
    public class ConstFieldKind : FieldKind
    {
        public string Define { get; set; }

        public ConstFieldKind(string name, IType type, BaseIdentiferKind parent) : base(name, type, parent)
        {
            parent.AddConstField(this);
        }

        public static ConstFieldKind NewConstFieldKind(NewFieldContext ctx, BaseIdentiferKind parent)
        {
            // 常量字段只能是int或者string
            if (ctx.Type is not IntType && ctx.Type is not StringType)
            {
                throw new TypeException("常量字段只能是int或者string");
            }
            return new ConstFieldKind(ctx.Name, ctx.Type, parent);
        }
    }
}