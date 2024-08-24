using System;
using Generator.Kind;
using Generator.Type;
using Generator.Util;

namespace Generator.Visitor
{
    public class ListenerTypeVisitor : ITypeVisitor
    {
        private readonly Writer m_Writer;
        private readonly string m_BeanFullName;
        private readonly string m_PropertiesName;
        private readonly string m_EventName;
        private readonly string m_IdFullName;
        private readonly Func<string, BaseIdentiferKind>? m_IdentiferFind;

        public ListenerTypeVisitor(Writer writer, string beanFullName, string propertiesName,
            string eventName, string idFullName, Func<string, BaseIdentiferKind>? finder = null)
        {
            m_Writer = writer;
            m_BeanFullName = beanFullName;
            m_PropertiesName = propertiesName;
            m_EventName = eventName;
            m_IdFullName = idFullName;
            m_IdentiferFind = finder;
        }

        public void Visit(StructType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ClassType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ByteType type)
        {
            VisitBase();
        }

        public void Visit(UShortType type)
        {
            VisitBase();
        }

        private void VisitBase()
        {
            m_Writer.WriteLine($"var v = ({m_BeanFullName})val;");
            m_Writer.WriteLine($"Event.Fire(new {m_EventName}(({m_IdFullName})key)");
            m_Writer.WriteLine($"{{");
            m_Writer.WriteLine($"    {m_PropertiesName} = v.{m_PropertiesName},");
            m_Writer.Write($"}});");
        }

        public void Visit(IntType type)
        {
            VisitBase();
        }

        public void Visit(UIntType type)
        {
            VisitBase();
        }

        public void Visit(LongType type)
        {
            VisitBase();
        }

        public void Visit(BoolType type)
        {
            VisitBase();
        }

        public void Visit(StringType type)
        {
            VisitBase();
        }

        public void Visit(FloatType type)
        {
            VisitBase();
        }

        public void Visit(DoubleType type)
        {
            VisitBase();
        }

        public void Visit(ArrayType type)
        {
            throw new NotImplementedException();
        }

        public void Visit(ListType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MapType type)
        {
            var valFullNameVisitor = new EdbFullNameTypeVisitor(m_IdentiferFind);
            type.Value().Accept(valFullNameVisitor);
            m_Writer.WriteLine($"var noteMap = note as NoteMap<{m_IdFullName}, {valFullNameVisitor.Result}>;");
            m_Writer.WriteLine($"foreach (var k in noteMap!.Added)");
            m_Writer.WriteLine($"{{");
            m_Writer.WriteLine($"    Event.Fire(new {m_EventName}(({m_IdFullName})key)");
            m_Writer.WriteLine(($"    {{"));
            m_Writer.WriteLine($"        IsAdd = true,");
            m_Writer.WriteLine($"        MKey = k,");
            m_Writer.WriteLine(($"    }});"));
            m_Writer.WriteLine($"}}");
            m_Writer.WriteLine($"foreach (var pair in noteMap!.Removed)");
            m_Writer.WriteLine($"{{");
            m_Writer.WriteLine($"    Event.Fire(new {m_EventName}(({m_IdFullName})key)");
            m_Writer.WriteLine(($"    {{"));
            m_Writer.WriteLine($"        MKey = pair.Key,");
            m_Writer.WriteLine(($"    }});"));
            m_Writer.WriteLine($"}}");
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