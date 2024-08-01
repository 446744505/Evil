using Generator.Util;

namespace Generator.Kind
{
    public abstract partial class BaseIdentiferKind
    {
        public string GenToString(int initTabN)
        {
            var writer = new Writer();
            writer.WriteLine(initTabN, "public override string ToString()");
            writer.WriteLine(initTabN, "{");
            writer.WriteLine(initTabN + 1, $"var sb = new System.Text.StringBuilder(\"{Name}{{\");");
            foreach (var fieldKind in Children())
            {
                writer.WriteLine(initTabN + 1, $"sb.Append(\"{fieldKind.Name}=\").Append({fieldKind.Name}).Append(\",\");");
            }
            writer.WriteLine(initTabN + 1, "sb.Append(\"}\");");
            writer.WriteLine(initTabN + 1, "return sb.ToString();");
            writer.WriteLine(initTabN, "}");
            return writer.ToString();
        }
    }
}