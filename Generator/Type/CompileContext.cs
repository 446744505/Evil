using Generator.Kind;

namespace Generator
{
    public class CompileContext
    {
        public Func<string, BaseIdentiferKind?> IdentiferFind { get; set; } = null!;
    }
}