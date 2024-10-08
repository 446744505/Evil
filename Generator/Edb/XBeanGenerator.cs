﻿
using System.IO;
using Generator.Context;
using Generator.Kind;
using Generator.Util;
using Generator.Visitor;

namespace Generator.Edb
{
    public class XBeanGenerator : BaseGenerator<XBeanNamespaceKind>
    {
        public XBeanGenerator(GloableContext gc) : 
            base(gc, new GeneratorContext(), Files.XBeanPath)
        {
        }

        protected override void Generate0()
        {
            foreach (var namespaceClassKind in NamespaceClassKinds())
            {
                var identiferKinds = namespaceClassKind.Value;
                foreach (var identiferKind in identiferKinds)
                {
                    var beanKind = (XBeanClassKind)identiferKind;
                    try
                    {
                        GenerateBean(beanKind);
                    } catch (System.Exception e)
                    {
                        throw new System.Exception($"生成XBean {beanKind.Name}失败", e);
                    }
                }
            }
        }
        
        private void GenerateBean(XBeanClassKind beanKind)
        {
            var fieldsLine = new Writer(true, 2);
            var getterLine = new Writer(true, 2);
            var constructorLine = new Writer(true, 3);
            var copyConstructorLine = new Writer(true, 3);
            var copyFromLine = new Writer(true, 3);
            var unmarshalLine = new Writer(true, 3);
            var marshalLine = new Writer(true, 3);
            var toProtoLine = new Writer();
            if (beanKind.IsProtoField) // 生成ToProto前半部分
            {
                toProtoLine.WriteLine();
                toProtoLine.WriteLine(2,$"public Proto.{beanKind.Name} ToProto()");
                toProtoLine.WriteLine(2,"{");
                toProtoLine.WriteLine(3,$"var _p_ = new Proto.{beanKind.Name}();");
            }
            foreach (var fk in beanKind.Children())
            {
                var fieldKind = (XBeanFieldKind)fk;
                // 字段定义
                var defineVisitor = new EdbFullNameTypeVisitor();
                fieldKind.Type.Accept(defineVisitor);
                if (beanKind.IdFieldName == fieldKind.Name) // bson id特性生成
                    fieldsLine.WriteLine("[MongoDB.Bson.Serialization.Attributes.BsonId]");
                else
                {
                    var addBsonAttrVisitor = new XBeanFieldAddBsonAttrTypeVisitor(beanKind, fieldKind);
                    fieldKind.Type.Accept(addBsonAttrVisitor);
                    fieldsLine.WriteLine(addBsonAttrVisitor.Result);
                }
                fieldsLine.WriteLine($"private {defineVisitor.Result} {fieldKind.Name};");
                
                // 构造函数生成
                var constructorVisitor = new EdbDefaultValueTypeVisitor(fieldKind);
                fieldKind.Type.Accept(constructorVisitor);
                if (!string.IsNullOrEmpty(constructorVisitor.Result))
                    constructorLine.WriteLine($"{fieldKind.Name} = {constructorVisitor.Result};");
                
                // 复制构造函数生成
                var copyConstructorVisitor = new CopyConstructorTypeVisitor(fieldKind, copyConstructorLine);
                fieldKind.Type.Accept(copyConstructorVisitor);
                
                // 复制函数生成
                var copyFromVisitor = new CopyFromTypeVisitor(fieldKind, copyFromLine);
                fieldKind.Type.Accept(copyFromVisitor);
                
                // unmarshal生成
                var unmarshalVisitor = new MongoUnmarshalTypeVisitor(fieldKind, beanKind.IdFieldName == fieldKind.Name, unmarshalLine);
                fieldKind.Type.Accept(unmarshalVisitor);
                
                // marshal生成
                var marshalVisitor = new MongoMarshalTypeVisitor(fieldKind, beanKind.IdFieldName == fieldKind.Name, marshalLine);
                fieldKind.Type.Accept(marshalVisitor);
                
                // toProto生成
                if (beanKind.IsProtoField && fieldKind.IsProtoField)
                {
                    var toProtoVisitor = new ToProtoTypeVisitor(fieldKind, toProtoLine, 3);
                    fieldKind.Type.Accept(toProtoVisitor);   
                }

                // getter 生成
                var getterVisitor = new XBeanFieldGetterTypeVisitor(beanKind, fieldKind);
                fieldKind.Type.Accept(getterVisitor);
                getterLine.WriteLine($"{getterVisitor.Result}");
            }
            
            if (beanKind.IsProtoField) // 生成ToProto后半部分
            {
                toProtoLine.WriteLine(3,"return _p_;");
                toProtoLine.WriteLine(2,"}");
            }
            
            var filePath = Path.Combine(OutPath, $"{beanKind.Name}{Files.CodeFileSuffix}");
            var code = $@"
{AutoGenerated}
#pragma warning disable 8669

using MongoDB.Bson;

namespace XBean
{{
    public class {beanKind.Name} : Edb.XBean, Edb.IMongoCodec<{beanKind.Name}>
    {{
        {fieldsLine}

        public {beanKind.Name}(Edb.XBean? _xp_, string _vn_) : base(_xp_, _vn_)
        {{
            {constructorLine}
        }}

        public {beanKind.Name}() : this(null, null!)
        {{
        }}

        public {beanKind.Name}({beanKind.Name} _o_) : this(_o_, null, null!)
        {{
        }}

        public {beanKind.Name}({beanKind.Name} _o_, Edb.XBean? _xp_, string _vn_) : base(_xp_, _vn_)
        {{
            _o_.VerifyStandaloneOrLockHeld(""_o_.{beanKind.Name}"", true);
            {copyConstructorLine}
        }}

        public void CopyFrom({beanKind.Name} _o_)
        {{
            _o_.VerifyStandaloneOrLockHeld(""CopyFrom{beanKind.Name}"", true);
            VerifyStandaloneOrLockHeld(""CopyTo{beanKind.Name}"", false);
            {copyFromLine}
        }}
        
        public {beanKind.Name} Unmarshal(BsonDocument _doc_)
        {{
            VerifyStandaloneOrLockHeld(""Unmarshal"", false);
            {unmarshalLine}
            return this;
        }}

        public BsonDocument Marshal(BsonDocument _doc_)
        {{
            VerifyStandaloneOrLockHeld(""Marshal"", true);
            {marshalLine}
            return _doc_;
        }}
{toProtoLine}
        {getterLine}

{beanKind.GenToString(2)}
    }}
}}
#pragma warning restore 8669
";
            Gc.Log($"生成文件: {filePath}");
            File.WriteAllText(filePath, code);
        }
    }
}