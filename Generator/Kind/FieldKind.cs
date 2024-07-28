using Generator.Context;
using Generator.Type;

namespace Generator.Kind
{
    public class FieldKind : BaseKind
    {
        #region 属性
        
        public string Name { get; }
        public IType Type { get; }
        public string? Comment { get; set; }
        
        #endregion
        
        public FieldKind(string name, IType type, IKind parent) : base(parent)
        {
            Name = name;
            Type = type;
        }

        protected override void Compile0(CompileContext ctx)
        {
            Type.Compile(ctx);
        }
    }
}