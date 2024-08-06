using System;
using Generator.Kind;
using Generator.Type;
using Generator.Util;

namespace Generator.Visitor
{
    public class MongoUnmarshalTypeVisitor : ITypeVisitor
    {
        private readonly XBeanFieldKind m_FieldKind;
        private readonly bool m_IsIdField;
        private readonly Writer m_Writer;
        private readonly CollectionType m_CollectionType = CollectionType.None;
        
        private string FieldName => m_FieldKind.Name;
        private string DocName => m_IsIdField ? "_id" : m_FieldKind.Name;

        public MongoUnmarshalTypeVisitor(XBeanFieldKind fieldKind, bool isIdField, Writer writer)
        {
            m_FieldKind = fieldKind;
            m_IsIdField = isIdField;
            m_Writer = writer;
        }
        
        private MongoUnmarshalTypeVisitor(XBeanFieldKind fieldKind, Writer writer, CollectionType collectionType)
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
                    m_Writer.WriteLine($@"{FieldName}.Unmarshal(doc[""{FieldName}""].AsBsonDocument);");
                    break;
                case CollectionType.List:
                    m_Writer.WriteLine($@"var _v_ = new {type.Name}(this, ""{FieldName}"");");
                    m_Writer.WriteLine(4,$"_v_.Unmarshal(_{FieldName}_[_i_].AsBsonDocument);");
                    m_Writer.Write(4, $"{FieldName}.Add(_v_);");
                    break;
                case CollectionType.Map:
                    var asVisitor = new MongoAsTypeVisitor();
                    var mapType = m_FieldKind.Type as MapType;
                    mapType!.Key().Accept(asVisitor);
                    m_Writer.WriteLine($@"var _v_ = new {type.Name}(this, ""{FieldName}"");");
                    m_Writer.WriteLine(4,"// 为了map的key可以是任意类型，所以map是以array的方式存入mongo的");
                    m_Writer.WriteLine(4, $"var _arr_ = _{FieldName}_[_i_].AsBsonArray;");
                    m_Writer.WriteLine(4,$"_v_.Unmarshal(_arr_[1].AsBsonDocument);");
                    m_Writer.Write(4, $"{FieldName}[_arr_[0].{asVisitor.Result}] = _v_;");
                    break;
            }
        }

        private void BaseVisit(IType type)
        {
            var asVisitor = new MongoAsTypeVisitor();
            switch (m_CollectionType)
            {
                case CollectionType.None:
                    type.Accept(asVisitor);
                    m_Writer.WriteLine($@"{FieldName} = doc[""{DocName}""].{asVisitor.Result};");
                    break;
                case CollectionType.List:
                    type.Accept(asVisitor);
                    m_Writer.WriteLine($"{FieldName}.Add(_{FieldName}_[_i_].{asVisitor.Result});");
                    break;
                case CollectionType.Map:
                    var keyAsVisitor = new MongoAsTypeVisitor();
                    var valAsVisitor = new MongoAsTypeVisitor();
                    var mapType = m_FieldKind.Type as MapType;
                    mapType!.Key().Accept(keyAsVisitor);
                    mapType.Value().Accept(valAsVisitor);
                    m_Writer.WriteLine(4,"// 为了map的key可以是任意类型，所以map是以array的方式存入mongo的");
                    m_Writer.WriteLine(4, $"var _arr_ = _{FieldName}_[_i_].AsBsonArray;");
                    m_Writer.Write(4, $"{FieldName}[_arr_[0].{keyAsVisitor.Result}] = _arr_[1].{valAsVisitor.Result};");
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
            var collectionVisitor = new MongoUnmarshalTypeVisitor(m_FieldKind, writer, CollectionType.List);
            type.Value().Accept(collectionVisitor);
            
            m_Writer.WriteLine($@"
            var _{FieldName}_ = doc[""{DocName}""].AsBsonArray;
            for (var _i_ = 0; _i_ < _{FieldName}_.Count; _i_++)
            {{
                {writer}
            }}
    ");
        }

        public void Visit(MapType type)
        {
            var writer = new Writer();
            var collectionVisitor = new MongoUnmarshalTypeVisitor(m_FieldKind, writer, CollectionType.Map);
            type.Value().Accept(collectionVisitor);
            
            m_Writer.WriteLine($@"
            var _{FieldName}_ = doc[""{DocName}""].AsBsonArray;
            for (var _i_ = 0; _i_ < _{FieldName}_.Count; _i_++)
            {{
                {writer}
            }}
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