using Generator.Type;

namespace Generator.Kind
{
    public class XBeanFieldKind : EdbFieldKind
    {
        /// <summary>
        /// 是协议字段，则要在ToProto方法中生成该字段的copy代码
        /// </summary>
        public bool IsProtoField { get; set; }
        
        public XBeanFieldKind(string name, IType type, IKind parent) : base(name, type, parent)
        {
        }
    }
}