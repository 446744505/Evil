using Evil.Util;
using Generator.Kind;
using Generator.Type;

namespace Generator.Visitor
{
    /// <summary>
    /// 生成xbean的字段定义
    /// </summary>
    public class XBeanFieldGetterTypeVisitor : ITypeVisitor
    {
        private readonly XBeanClassKind m_BeanKind;
        private readonly XBeanFieldKind m_FieldKind;
        
        private string FieldName => m_FieldKind.Name;
        public string Result { get; set; }
        
        public XBeanFieldGetterTypeVisitor(XBeanClassKind beanKind, XBeanFieldKind fieldKind)
        {
            m_BeanKind = beanKind;
            m_FieldKind = fieldKind;
        }

        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            var fullNameVisitor = new EdbFullNameTypeVisitor();
            type.Accept(fullNameVisitor);
            var attrName = FieldName.FirstCharToUpper();
            Result = $@"[MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public {fullNameVisitor.Result} {attrName} {{
            get
            {{
                VerifyStandaloneOrLockHeld(""Get{attrName}"", true);
                return {FieldName};
            }}
        }}
";
        }

        public void Visit(ByteType type)
        {
            BaseVisit(type);
        }

        public void Visit(UShortType type)
        {
            BaseVisit(type);
        }

        public void BaseVisit(IType type)
        {
            var fullNameVisitor = new EdbFullNameTypeVisitor();
            type.Accept(fullNameVisitor);
            var attrName = FieldName.FirstCharToUpper();
            Result = $@"[MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public {fullNameVisitor.Result} {attrName} {{
            get
            {{
                VerifyStandaloneOrLockHeld(""Get{attrName}"", true);
                return {FieldName};
            }}
            set
            {{
                VerifyStandaloneOrLockHeld(""Set{attrName}"", false);
                Edb.Logs.LogObject(this, ""{FieldName}"");
                {FieldName} = value;
            }}
        }}
";
        }

        public void Visit(IntType type)
        {
            BaseVisit(type);
        }

        public void Visit(UIntType type)
        {
            BaseVisit(type);
        }

        public void Visit(LongType type)
        {
            BaseVisit(type);
        }

        public void Visit(BoolType type)
        {
            BaseVisit(type);
        }

        public void Visit(StringType type)
        {
            BaseVisit(type);
        }

        public void Visit(FloatType type)
        {
            BaseVisit(type);
        }

        public void Visit(DoubleType type)
        {
            BaseVisit(type);
        }

        public void Visit(ArrayType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ListType type)
        {
            var fullNameVisitor = new EdbFullNameTypeVisitor();
            type.Accept(fullNameVisitor);
            var valueVisitor = new FullNameTypeVisitor();
            type.Value().Accept(valueVisitor);
            var attrName = FieldName.FirstCharToUpper();
            Result = $@"[MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public {fullNameVisitor.Result} {attrName} {{
            get
            {{
                if (!Edb.Transaction.IsActive)
                    return {FieldName};
                return Edb.Logs.LogList<{valueVisitor.Result}>(this, ""{FieldName}"", () => {{
                    VerifyStandaloneOrLockHeld(""Get{attrName}"", true);
                }});
            }}
        }}
";
        }

        public void Visit(MapType type)
        {
            var fullNameVisitor = new EdbFullNameTypeVisitor();
            type.Accept(fullNameVisitor);
            var keyVisitor = new FullNameTypeVisitor();
            var valueVisitor = new FullNameTypeVisitor();
            type.Key().Accept(keyVisitor);
            type.Value().Accept(valueVisitor);
            var attrName = FieldName.FirstCharToUpper();
            Result = $@"[MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public {fullNameVisitor.Result} {attrName} {{
            get
            {{
                if (!Edb.Transaction.IsActive)
                    return {FieldName};
                return Edb.Logs.LogMap<{keyVisitor.Result}, {valueVisitor.Result}>(this, ""{FieldName}"", () => {{
                    VerifyStandaloneOrLockHeld(""Get{attrName}"", true);
                }});
            }}
        }}
";
        }

        public void Visit(TaskType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(WaitCompileIdentiferType type)
        {
            throw new System.NotImplementedException();
        }
    }
}