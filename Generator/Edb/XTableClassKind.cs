using System;
using Generator.Type;

namespace Generator.Kind
{
    public class XTableClassKind : ClassKind
    {
        public string LockName { get; set; }
        public int Capacity { get; set; }
        public bool IsMemory { get; set; }
        public string IdFieldName { get; set; }
        public XTableClassKind(BaseIdentiferType type, IKind parent) : base(type, parent)
        {
        }
        
        public XTableFieldKind FindIdField()
        {
            foreach (var field in Children())
            {
                if (field.Name == IdFieldName)
                {
                    return (XTableFieldKind)field;
                }
            }

            throw new System.Exception($"表{Name}没有id字段");
        }
    }
}