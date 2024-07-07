using System.Collections.Generic;
using System.IO;
using Generator.Context;
using Generator.Kind;
using Generator.Util;
using Generator.Visitor;

namespace Generator.Proto
{
    public class ProtoGenerator
    {
        private readonly GloableContext m_Gc;

        public ProtoGenerator(GloableContext gc)
        {
            m_Gc = gc;
        }

        public void GenerateMeta(GloableContext gc)
        {
            var context = new ProtoContext();
            context.IdentiferFind = name =>
            {
                return m_Gc.FindIdentiferKind(name);
            };
            // 清空并创建临时文件夹
            var tmpPath = Path.Combine(m_Gc.OutPath, Files.ProtoPath);
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }
            Directory.CreateDirectory(tmpPath);
            
            // 从context中获取所有的namespace和class
            Dictionary<string, List<BaseIdentiferKind>> namespaceClassKinds = new();
            foreach (var fc in gc.FileContexts)
            {
                foreach (var namespaceKind in fc.NamespaceKinds)
                {
                    if (namespaceClassKinds.TryGetValue(namespaceKind.Name, out var classKinds))
                    {
                        classKinds.AddRange(namespaceKind.Children());
                    }
                    else
                    {
                        namespaceClassKinds.Add(namespaceKind.Name, namespaceKind.Children());
                    }
                }
            }
            // 遍历所有namespace，每个namespace生成一个proto文件
            foreach (var namespaceClassKind in namespaceClassKinds)
            {
                var namespaceName = namespaceClassKind.Key;
                var identiferKinds = namespaceClassKind.Value;
                var writer = new Writer();
                MakeHead(writer, namespaceName);
                foreach (var identiferKind in identiferKinds)
                {
                    MakeMessage(writer, identiferKind, context);
                }
                CreateFile(writer, namespaceName);
            }
        }

        private void MakeMessage(Writer writer, BaseIdentiferKind identiferKind, ProtoContext ctx)
        {
            writer.WriteLine("message " + identiferKind.Name + " {");
            foreach (var field in identiferKind.Children())
            {
                var fieldVisitor = new ProtoFieldTypeVisitor(field, ctx);
                field.Type.Accept(fieldVisitor);
                writer.WriteLine(4, fieldVisitor.Context.Line);
            }
            writer.WriteLine("}");
        }

        private void MakeHead(Writer writer, string namespaceName)
        {
            writer.WriteLine(@"syntax = ""proto3"";");
            writer.WriteLine();
            writer.WriteLine("package " + namespaceName + ";");
            writer.WriteLine();
        }
        
        private void CreateFile(Writer writer, string namespaceName)
        {
            var fileName = CalProtoFileNameByNamespace(namespaceName);
            var outPath = Path.Combine(m_Gc.OutPath, Files.ProtoPath, fileName);
            File.WriteAllText(outPath, writer.ToString());
            m_Gc.Log($"生成文件:{outPath}");
        }
       
        private string CalProtoFileNameByNamespace(string name)
        {
            // 将.换成_，且转换为小写
            return name.Replace(".", "_").ToLower() + ".proto";
        }
    }
}