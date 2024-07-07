using System;
using Generator.Kind;

namespace Generator.Proto
{
    public class ProtoContext
    {
        public Func<string, BaseIdentiferKind?> IdentiferFind { get; set; } = null!;
    }
}