using System.Collections.Generic;
using Generator.Kind;
using Generator.Type;
using Generator.Util;
using Generator.Visitor;

namespace Generator.Proto
{
    public class ProtoImportTypeVisitorContext : ITypeVisitorContext
    {
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
    }
    public class ProtoImportTypeVisitor : BaseTypeVisitor<ProtoImportTypeVisitorContext>
    {
        private readonly ProtoContext m_Pc;
        public ProtoImportTypeVisitor(ProtoContext pc) : base(new ProtoImportTypeVisitorContext())
        {
            m_Pc = pc;
        }

        public override void Visit(StructType type)
        {
            var identiferKind = m_Pc.IdentiferFind.Invoke(type.Name);
            var namespaceKind = identiferKind.Parent() as NamespaceKind;
            Context.AddImport($"{ProtoUtil.CalProtoFileNameByNamespace(namespaceKind!.Name)}");
        }

        public override void Visit(ClassType type)
        {
            
            var identiferKind = m_Pc.IdentiferFind.Invoke(type.Name);
            var namespaceKind = identiferKind.Parent() as NamespaceKind;
            Context.AddImport($"{ProtoUtil.CalProtoFileNameByNamespace(namespaceKind!.Name)}");
        }

        public override void Visit(IntType type)
        {
        }

        public override void Visit(LongType type)
        {
        }

        public override void Visit(BoolType type)
        {
            
        }

        public override void Visit(StringType type)
        {
        }

        public override void Visit(FloatType type)
        {
        }

        public override void Visit(DoubleType type)
        {
        }

        public override void Visit(ListType type)
        {
            var importVisitor = new ProtoImportTypeVisitor(m_Pc);
            type.Value().Accept(importVisitor);
            if (importVisitor.Context.Imports.Count > 0)
            {
                Context.AddImports(importVisitor.Context.Imports);
            }
        }

        public override void Visit(MapType type)
        {
            var keyImportVisitor = new ProtoImportTypeVisitor(m_Pc);
            type.Key().Accept(keyImportVisitor);
            Context.AddImports(keyImportVisitor.Context.Imports);
            var valImportVisitor = new ProtoImportTypeVisitor(m_Pc);
            type.Value().Accept(valImportVisitor);
            Context.AddImports(valImportVisitor.Context.Imports);
        }
    }
}