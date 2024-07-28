using System;
using Generator.Kind;

namespace Generator.Context
{
    public class GeneratorContext
    {
        public Func<string, BaseIdentiferKind> IdentiferFind { get; set; } = null!;
    }
}