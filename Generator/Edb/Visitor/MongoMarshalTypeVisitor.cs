using System;
using Generator.Kind;
using Generator.Type;
using Generator.Util;

namespace Generator.Visitor
{
    public class MongoMarshalTypeVisitor : ITypeVisitor
    {
        private readonly XBeanFieldKind m_FieldKind;
        private readonly bool m_IsIdField;
        private readonly Writer m_Writer;
        private readonly CollectionType m_CollectionType = CollectionType.None;
        
        private string FieldName => m_FieldKind.Name;
        private string DocName => m_IsIdField ? "_id" : m_FieldKind.Name;

        public MongoMarshalTypeVisitor(XBeanFieldKind fieldKind, bool isIdField, Writer writer)
        {
            m_FieldKind = fieldKind;
            m_IsIdField = isIdField;
            m_Writer = writer;
        }
        
        private MongoMarshalTypeVisitor(XBeanFieldKind fieldKind, Writer writer, CollectionType collectionType)
        {
            m_FieldKind = fieldKind;
            m_Writer = writer;
            m_CollectionType = collectionType;
        }

        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            switch (m_CollectionType)
            {
                case CollectionType.None:
                    m_Writer.WriteLine($"_doc_[\"{DocName}\"] = {FieldName}.Marshal(new BsonDocument());");
                    break;
                case CollectionType.List:
                    m_Writer.Write($"_{FieldName}_.Add(_v_.Marshal(new BsonDocument()));");
                    break;
                case CollectionType.Map:
                    m_Writer.Write($"_arr_.Add(_pair_.Value.Marshal(new BsonDocument()));");
                    break;
            }
        }

        private void BaseVisit(IType type)
        {
            switch (m_CollectionType)
            {
                case CollectionType.None:
                    m_Writer.WriteLine($@"_doc_[""{DocName}""] = {FieldName};");
                    break;
                case CollectionType.List:
                    m_Writer.WriteLine($@"_{FieldName}_.Add(_v_);");
                    break;
                case CollectionType.Map:
                    m_Writer.WriteLine($@"_arr_.Add(_pair_.Value);");
                    break;
            }
        }

        public void Visit(IntType type)
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
            throw new NotSupportedException("MongoDB does not support float type");
        }

        public void Visit(DoubleType type)
        {
            BaseVisit(type);
        }

        public void Visit(ListType type)
        {
            var writer = new Writer();
            var collectionVisitor = new MongoMarshalTypeVisitor(m_FieldKind, writer, CollectionType.List);
            type.Value().Accept(collectionVisitor);
            
            m_Writer.WriteLine($@"
            var _{FieldName}_ = new BsonArray({FieldName}.Count);
            foreach (var _v_ in {FieldName})
            {{
                {writer}
            }}
            _doc_[""{DocName}""] = _{FieldName}_;
    ");
        }

        public void Visit(MapType type)
        {
            var writer = new Writer();
            var collectionVisitor = new MongoMarshalTypeVisitor(m_FieldKind, writer, CollectionType.Map);
            type.Value().Accept(collectionVisitor);
            
            m_Writer.WriteLine($@"
            var _{FieldName}_ = new BsonArray({FieldName}.Count);
            foreach (var _pair_ in {FieldName})
            {{
                var _arr_ = new BsonArray(2);
                _arr_.Add(_pair_.Key);
                {writer}
                _{FieldName}_.Add(_arr_);
            }}
            _doc_[""{DocName}""] = _{FieldName}_;
    ");
        }

        public void Visit(TaskType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(WaitCompileIdentiferType type)
        {
            throw new System.NotImplementedException();
        }
        
        private enum CollectionType
        {
            None,
            List,
            Map
        }
    }
}