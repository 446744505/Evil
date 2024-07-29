using System.Collections.Generic;
using System.IO;
using Generator.Context;
using Generator.Kind;
using Generator.Util;
using Generator.Visitor;
using Google.Protobuf.Reflection;

namespace Generator.Proto
{
    public class ProtoGenerator : BaseGenerator<ProtoNamespaceKind>
    {

        public ProtoGenerator(GloableContext gc) : base(gc, new GeneratorContext(), Files.ProtoPath)
        {
        }
        
        /// <summary>
        /// 生成所有.proto文件和对应的C#文件
        /// </summary>
        /// <param name="gc"></param>
        protected override void Generate0()
        {
            var context = Context;
            
            // 遍历所有namespace，每个namespace生成一个proto文件
            var fileSet = new FileDescriptorSet();
            fileSet.AddImportPath(OutPath);
            foreach (var namespaceClassKind in NamespaceClassKinds())
            {
                var namespaceName = namespaceClassKind.Key;
                var identiferKinds = namespaceClassKind.Value;
                var writer = new Writer();
                MakeHead(writer, namespaceName);
                MakeImport(writer, identiferKinds, namespaceName, context);
                foreach (var identiferKind in identiferKinds)
                {
                    MakeMessage(writer, identiferKind, context);
                    writer.WriteLine();
                }
                var fileName = CreateFile(writer, namespaceName);
                fileSet.Add(fileName);
            }

            CreateCodeFile(fileSet);
        }

        private void CreateCodeFile(FileDescriptorSet fileSet)
        {
            fileSet.Process();
            var generator = new ProtoCodeGenerator();
            foreach (var codeFile in generator.Generate(fileSet))
            {
                File.WriteAllText(Path.Combine(Gc.OutPath, Files.ProtoPath, codeFile.Name), codeFile.Text);
                Gc.Log("生成文件:" + codeFile);
            }
        }

        private void MakeImport(Writer writer, List<BaseIdentiferKind> identiferKinds, 
            string namespaceName, GeneratorContext context)
        {
            List<string> importList = new();
            var self = ProtoUtil.CalProtoFileNameByNamespace(namespaceName);
            foreach (var identiferKind in identiferKinds)
            {
                foreach (var fieldKind in identiferKind.Children())
                {
                    var importVisitor = new ProtoImportTypeVisitor(context);
                    fieldKind.Type.Accept(importVisitor);
                    foreach (var import in importVisitor.Imports)
                    {
                        if (import != self && !importList.Contains(import))
                        {
                            importList.Add($"import \"{import}\";");   
                        }
                    }
                }
            }
           
            foreach (var import in importList)
            {
                writer.WriteLine(import);
            }
            if (importList.Count > 0)
            {
                writer.WriteLine();
            }
        }

        private void MakeMessage(Writer writer, BaseIdentiferKind identiferKind, GeneratorContext ctx)
        {
            // 注释
            if (!string.IsNullOrWhiteSpace(identiferKind.Comment))
            {
                writer.WriteLine($"// {identiferKind.Comment}");   
            }
            writer.WriteLine("message " + identiferKind.Name + " {");
            foreach (var field in identiferKind.Children())
            {
                var fieldVisitor = new ProtoFieldTypeVisitor(field, ctx);
                field.Type.Accept(fieldVisitor);
                writer.WriteLine(1, fieldVisitor.Result);
            }
            writer.WriteLine("}");
        }

        private void MakeHead(Writer writer, string namespaceName)
        {
            writer.WriteLine(@"syntax = ""proto3"";");
            writer.WriteLine();
            writer.WriteLine($"package {namespaceName};");
            writer.WriteLine();
        }
        
        private string CreateFile(Writer writer, string namespaceName)
        {
            var fileName = ProtoUtil.CalProtoFileNameByNamespace(namespaceName);
            var outPath = Path.Combine(Gc.OutPath, Files.ProtoPath, fileName);
            File.WriteAllText(outPath, writer.ToString());
            Gc.Log($"生成文件:{outPath}");
            return fileName;
        }
    }
}