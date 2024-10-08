using Generator.Util;
using Generator.Visitor;

namespace Generator.Kind
{
    public abstract partial class BaseIdentiferKind
    {
        public string GenConstFields(int initTabN)
        {
            if (m_ConstFields.Count == 0)
                return string.Empty;
            
            var writer = new Writer();
            foreach (var field in m_ConstFields)
            {
                writer.WriteLine(initTabN, $"{field.Define}");
            }
            return  writer.ToString();
        }
        
        public string GenToString(int initTabN)
        {
            if (Children().Count == 0)
                return string.Empty;
            
            var writer = new Writer();
            writer.WriteLine(initTabN, "public override string ToString()");
            writer.WriteLine(initTabN, "{");
            writer.WriteLine(initTabN + 1, $"var sb = new System.Text.StringBuilder(\"{Name}{{\");");
            foreach (var fieldKind in Children())
            {
                var visitor = new ToStringTypeVisitor(fieldKind);
                fieldKind.Type.Accept(visitor);
                writer.WriteLine(initTabN + 1, $"sb.Append(\"{fieldKind.Name}=\").Append({visitor.Result}).Append(\",\");");
            }
            writer.WriteLine(initTabN + 1, "sb.Append(\"}\");");
            writer.WriteLine(initTabN + 1, "return sb.ToString();");
            writer.WriteLine(initTabN, "}");
            return writer.ToString();
        }
    }
}