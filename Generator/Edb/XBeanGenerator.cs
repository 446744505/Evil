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
            foreach (var fieldKind in beanKind.Children())
            {
                // 字段定义
                var defineVisitor = new EdbFullNameTypeVisitor();
                fieldKind.Type.Accept(defineVisitor);
                fieldsLine.WriteLine($"private {defineVisitor.Result} {fieldKind.Name};");
                
                // 构造函数生成
                var constructorVisitor = new EdbDefaultValueTypeVisitor((XBeanFieldKind)fieldKind);
                fieldKind.Type.Accept(constructorVisitor);
                if (!string.IsNullOrEmpty(constructorVisitor.Result))
                    constructorLine.WriteLine($"{fieldKind.Name} = {constructorVisitor.Result};");
                
                // 复制构造函数生成
                var copyConstructorVisitor = new CopyConstructorTypeVisitor((XBeanFieldKind)fieldKind, copyConstructorLine);
                fieldKind.Type.Accept(copyConstructorVisitor);
                
                // 复制函数生成
                var copyFromVisitor = new CopyFromTypeVisitor((XBeanFieldKind)fieldKind, copyFromLine);
                fieldKind.Type.Accept(copyFromVisitor);

                // getter 生成
                if (beanKind.IdFieldName == fieldKind.Name) // bson id特性生成
                    getterLine.WriteLine("[MongoDB.Bson.Serialization.Attributes.BsonId]");
                var getterVisitor = new XBeanFieldGetterTypeVisitor(beanKind, (XBeanFieldKind)fieldKind);
                fieldKind.Type.Accept(getterVisitor);
                getterLine.WriteLine($"{getterVisitor.Result}");
            }
            
            var filePath = Path.Combine(OutPath, $"{beanKind.Name}{Files.CodeFileSuffix}");
            var code = $@"
{AutoGenerated}
namespace XBean
{{
    public class {beanKind.Name} : Edb.XBean
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
        
        {getterLine}
    }}
}}
";
            Gc.Log($"生成文件: {filePath}");
            File.WriteAllText(filePath, code);
        }
    }
}