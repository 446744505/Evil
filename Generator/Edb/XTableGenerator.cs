﻿using System.Collections.Generic;
using System.IO;
using Evil.Util;
using Generator.Context;
using Generator.Kind;
using Generator.Util;
using Generator.Visitor;

namespace Generator.Edb
{
    public class XTableGenerator : BaseGenerator<XTableNamespaceKind>
    {
        public XTableGenerator(GloableContext gc) : 
            base(gc, new GeneratorContext(), Files.XTablePath)
        {
        }

        protected override void Generate0()
        {
            var tables = new List<string>();
            foreach (var namespaceClassKind in NamespaceClassKinds())
            {
                var identiferKinds = namespaceClassKind.Value;
                foreach (var identiferKind in identiferKinds)
                {
                    var tableKind = (XTableClassKind)identiferKind;
                    try
                    {
                        GenerateTable(tableKind);
                    }
                    catch (System.Exception e)
                    {
                        throw new System.Exception($"生成XTable {tableKind.Name}失败", e);
                    }

                    tables.Add(tableKind.Name);
                }
            }
            GenerateTables(tables);
        }

        private void GenerateTables(List<string> tables)
        {
            if (tables.Count == 0)
            {
                return;
            }
            
            var instanceLine = new Writer(true, 3);
            var newInstanceLine = new Writer(true, 2);
            foreach (var table in tables)
            {
                instanceLine.WriteLine($"{table},");
                newInstanceLine.WriteLine($"internal static readonly Table.{table} {table} = new();");
            }
            var filePath = Path.Combine(OutPath, $"Tables{Files.CodeFileSuffix}");
            var code = $@"
{AutoGenerated}
using System.Collections.Generic;
using Edb;

namespace XTable
{{
    public static class Tables
    {{
        {newInstanceLine}
        public static readonly List<BaseTable> All = new()
        {{
            {instanceLine}
        }};
    }}
}}
";
            Gc.Log($"生成文件: {filePath}");
            File.WriteAllText(filePath, code);
        }

        private void GenerateTable(XTableClassKind tableKind)
        {
            var filePath = Path.Combine(OutPath, $"{tableKind.Name}{Files.CodeFileSuffix}");
            var idField = tableKind.FindIdField();
            var idFullNameVisitor = new FullNameTypeVisitor();
            idField.Type.Accept(idFullNameVisitor);
            var idFullName = idFullNameVisitor.Result;
            var valueFullName = $"{Namespaces.XBeanNamespace}.{tableKind.Name}";
            var code = $@"
{AutoGenerated}

using Edb;
using MongoDB.Bson;

namespace Table
{{
    internal class {tableKind.Name} : TTable<{idFullName}, {valueFullName}>
    {{
        public override string Name => ""{tableKind.Name}"";
        public override TableConfig Config => new()
        {{
            Name = ""{tableKind.Name}"",
            CacheCapacity = {tableKind.Capacity},
            IsMemory = {tableKind.IsMemory.ToString().ToLower()},
            Lock = ""{tableKind.LockName}"",
        }};

        public override {valueFullName} NewValue()
        {{
            return new {valueFullName}();
        }}

        public override {idFullName} MarshalKey({idFullName} key)
        {{
            return key;
        }}

        public override BsonDocument MarshalValue({valueFullName} value)
        {{
            return value.Marshal(new BsonDocument());
        }}

        public override {valueFullName} UnmarshalValue(BsonDocument value)
        {{
            return new {valueFullName}().Unmarshal(value);
        }}

        public {valueFullName} Select({idFullName} key)
        {{
            return Get(key, false);
        }}

        public bool Insert({valueFullName} value)
        {{
            return Add(value.{idField.Name.FirstCharToUpper()}, value);
        }}

        public bool Delete({idFullName} key)
        {{
            return Remove(key);
        }}

        public {valueFullName} Update({idFullName} key)
        {{
            return Get(key, true);
        }}
    }}
}}

namespace XTable
{{
    public static class {tableKind.Name}
    {{
        public static {valueFullName} Select({idFullName} key)
        {{
            return Tables.{tableKind.Name}.Select(key);
        }}

        public static bool Insert({valueFullName} value)
        {{
            return Tables.{tableKind.Name}.Insert(value);
        }}

        public static bool Delete({idFullName} key)
        {{
            return Tables.{tableKind.Name}.Delete(key);
        }}

        public static {valueFullName} Update({idFullName} key)
        {{
            return Tables.{tableKind.Name}.Update(key);
        }}
    }}
}}
";
            Gc.Log($"生成文件: {filePath}");
            File.WriteAllText(filePath, code);
        }
    }
}