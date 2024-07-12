using Generator.Kind;
using Generator.Type;

namespace Generator.Proto;

public class ReqClassKind : ClassKind
{
    public string AckFullName { get; set; }
    public ReqClassKind(BaseIdentiferType type, IKind parent) : base(type, parent)
    {
    }
}