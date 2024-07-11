using System.Collections.Generic;
using Generator.Kind;
using Generator.Type;
using Generator.Util;
using Generator.Visitor;

namespace Generator.Proto
{
    public class ProtoImportTypeVisitor : ITypeVisitor
    {
        private readonly ProtoContext m_Pc;
        private readonly List<string> m_Imports = new();
        public IReadOnlyList<string> Imports => m_Imports;
        
        public void AddImport(string import)
        {
            if (!m_Imports.Contains(import))
            {
                m_Imports.Add(import);   
            }
        }
        
        public void AddImports(IReadOnlyList<string> imports)
        {
            foreach (var import in imports)
            {
                AddImport(import);
            }
        }
        public ProtoImportTypeVisitor(ProtoContext pc)
        {
            m_Pc = pc;
        }

        public void Visit(StructType type)
        {
            var identiferKind = m_Pc.IdentiferFind.Invoke(type.Name);
            var namespaceKind = identiferKind.Parent() as NamespaceKind;
            AddImport($"{ProtoUtil.CalProtoFileNameByNamespace(namespaceKind!.Name)}");
        }

        public void Visit(ClassType type)
        {
            
            var identiferKind = m_Pc.IdentiferFind.Invoke(type.Name);
            var namespaceKind = identiferKind.Parent() as NamespaceKind;
            AddImport($"{ProtoUtil.CalProtoFileNameByNamespace(namespaceKind!.Name)}");
        }

        public void Visit(IntType type)
        {
        }

        public void Visit(LongType type)
        {
        }

        public void Visit(BoolType type)
        {
            
        }

        public void Visit(StringType type)
        {
        }

        public void Visit(FloatType type)
        {
        }

        public void Visit(DoubleType type)
        {
        }

        public void Visit(ListType type)
        {
            var importVisitor = new ProtoImportTypeVisitor(m_Pc);
            type.Value().Accept(importVisitor);
            AddImports(importVisitor.Imports);
        }

        public void Visit(MapType type)
        {
            var keyImportVisitor = new ProtoImportTypeVisitor(m_Pc);
            type.Key().Accept(keyImportVisitor);
            AddImports(keyImportVisitor.Imports);
            var valImportVisitor = new ProtoImportTypeVisitor(m_Pc);
            type.Value().Accept(valImportVisitor);
            AddImports(valImportVisitor.Imports);
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