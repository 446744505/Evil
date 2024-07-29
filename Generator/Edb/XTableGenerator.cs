using System.Collections.Generic;
using System.IO;
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
                    GenerateTable(tableKind);
                    tables.Add(tableKind.Name);
                }
            }
            GenerateTables(tables);
        }

        private void GenerateTables(List<string> tables)
        {
            var instanceLine = new Writer();
            var newInstanceLine = new Writer();
            foreach (var table in tables)
            {
                instanceLine.WriteLine($"{table},");
                newInstanceLine.WriteLine($"internal static readonly Table.{table} {table} = new();");
            }
            var filePath = Path.Combine(OutPath, $"Tables{Files.CodeFileSuffix}");
            var code = $@"
using System.Collections.Generic;
using Edb;

namespace XTable
{{
    public static class Tables
    {{
        public static readonly List<BaseTable> All = new()
        {{
            {instanceLine}
        }};
        {newInstanceLine}
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
            var fullNameVisitor = new FullNameTypeVisitor();
            idField.Type.Accept(fullNameVisitor);
            var idFullName = fullNameVisitor.Result;
            var valueFullName = $"XBean.{tableKind.Name}";
            var code = $@"
using System.Threading.Tasks;

using Edb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

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
            return value.ToBsonDocument();
        }}

        public override {valueFullName} UnmarshalValue(BsonDocument value)
        {{
            return BsonSerializer.Deserialize<{valueFullName}>(value);
        }}

        public async Task<{valueFullName}?> Select({idFullName} key)
        {{
            return await GetAsync(key, false);
        }}

        public async Task<bool> Insert({valueFullName} value)
        {{
            return await AddAsync(value.{idField.Name}, value);
        }}

        public async Task<bool> Delete({idFullName} key)
        {{
            return await RemoveAsync(key);
        }}

        public async Task<{valueFullName}?> Update({idFullName} key)
        {{
            return await GetAsync(key, true);
        }}
    }}
}}

namespace XTable
{{
    public static class {tableKind.Name}
    {{
        public static async Task<{valueFullName}?> Select({idFullName} key)
        {{
            return await Tables.{tableKind.Name}.Select(key);
        }}

        public static async Task<bool> Insert({valueFullName} value)
        {{
            return await Tables.{tableKind.Name}.Insert(value);
        }}

        public static async Task<bool> Delete({idFullName} key)
        {{
            return await Tables.{tableKind.Name}.Delete(key);
        }}

        public static async Task<{valueFullName}?> Update({idFullName} key)
        {{
            return await Tables.{tableKind.Name}.Update(key);
        }}
    }}
}}
";
            Gc.Log($"生成文件: {filePath}");
            File.WriteAllText(filePath, code);
        }
    }
}