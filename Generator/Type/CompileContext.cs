using System;
using Generator.Kind;

namespace Generator.Context
{
    public class CompileContext
    {
        public Func<string, BaseIdentiferKind> IdentiferFind { get; set; } = null!;
    }
}