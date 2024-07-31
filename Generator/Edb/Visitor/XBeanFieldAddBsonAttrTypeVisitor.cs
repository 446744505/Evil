using Generator.Kind;
using Generator.Type;

namespace Generator.Visitor
{
    public class XBeanFieldAddBsonAttrTypeVisitor : ITypeVisitor
    {
        private readonly XBeanClassKind m_BeanKind;
        private readonly XBeanFieldKind m_FieldKind;

        public XBeanFieldAddBsonAttrTypeVisitor(XBeanClassKind beanKind, XBeanFieldKind fieldKind)
        {
            m_BeanKind = beanKind;
            m_FieldKind = fieldKind;
        }
        
        public string Result { get; set; }

        private void SimpleVisit()
        {
            Result = "[MongoDB.Bson.Serialization.Attributes.BsonElement]";
        }
        
        public void Visit(StructType type)
        {
            SimpleVisit();
        }

        public void Visit(ClassType type)
        {
            SimpleVisit();
        }

        public void Visit(IntType type)
        {
            SimpleVisit();
        }

        public void Visit(LongType type)
        {
            SimpleVisit();
        }

        public void Visit(BoolType type)
        {
            SimpleVisit();
        }

        public void Visit(StringType type)
        {
            SimpleVisit();
        }

        public void Visit(FloatType type)
        {
            SimpleVisit();
        }

        public void Visit(DoubleType type)
        {
            SimpleVisit();
        }

        public void Visit(ListType type)
        {
            SimpleVisit();
        }

        public void Visit(MapType type)
        {
            // map使用数组的方式序列化(如果用字典的方式序列化,要求key必须是string类型,烦得很)
            Result = "[MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]";
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