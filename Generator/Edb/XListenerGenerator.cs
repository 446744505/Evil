﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Evil.Util;
using Generator.Context;
using Generator.Kind;
using Generator.Util;
using Generator.Visitor;

namespace Generator.Edb
{
    public class XListenerGenerator : BaseGenerator<XTableNamespaceKind>
    {
        public XListenerGenerator(GloableContext gc) : 
            base(gc, new GeneratorContext(), Files.XListenerPath)
        {
        }

        protected override void Generate0()
        {
            foreach (var namespaceClassKind in NamespaceClassKinds())
            {
                var identiferKinds = namespaceClassKind.Value;
                foreach (var identiferKind in identiferKinds)
                {
                    var tableKind = (XTableClassKind)identiferKind;
                    try
                    {
                        var it = tableKind.Children()
                            .Select(filed => (XTableFieldKind)filed)
                            .Where(filed => filed.IsListenerField);
                        foreach (var fieldKind in it)
                        {
                            GenerateListener(tableKind, fieldKind);
                        }
                    }
                    catch (System.Exception e)
                    {
                        throw new System.Exception($"生成XListener {tableKind.Name}失败", e);
                    }
                }
            }
        }

        private void GenerateListener(XTableClassKind tableKind, XTableFieldKind fieldKind)
        {
            var propertiesName = fieldKind.Name.FirstCharToUpper();
            var listenerName = $"{tableKind.Name}{propertiesName}Listener";
            var eventName = $"{tableKind.Name}{propertiesName}Event";
            var filePath = Path.Combine(OutPath, $"{listenerName}{Files.CodeFileSuffix}");
            var idField = tableKind.FindIdField();
            var idFullNameVisitor = new FullNameTypeVisitor();
            idField.Type.Accept(idFullNameVisitor);
            var idFullName = idFullNameVisitor.Result;
            var valFullNameVisitor = new FullNameTypeVisitor();
            fieldKind.Type.Accept(valFullNameVisitor);
            var valFullName = valFullNameVisitor.Result;
            var beanFullName = $"XBean.{tableKind.Name}";
            var code = $@"
{AutoGenerated}
using System.Threading.Tasks;

using Edb;
using Evil.Event;

namespace XListener
{{
    public class {listenerName} : IListener
    {{
        public Task OnChanged(object key, object val, string varName, INote? note)
        {{
            var v = ({beanFullName})val;
            Event.Fire(new {eventName}(({idFullName})key)
            {{
                {propertiesName} = v.{propertiesName},
            }});
            return Task.CompletedTask;
        }}
    }}

    public class {eventName} : EEvent<{idFullName}>
    {{
        public {valFullName} {propertiesName} {{ get; set; }}

        public {eventName}({idFullName} key) : base(key)
        {{
        }}
    }}
}}
";
            Gc.Log($"生成文件: {filePath}");
            File.WriteAllText(filePath, code);
        }
    }
}