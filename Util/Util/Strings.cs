using System.Text;

namespace Evil.Util
{
    public class Strings
    {
        public static string ToCustomString<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            var sb = new StringBuilder("{");
            foreach (var (key, value) in dict)
            {
                sb.Append(key).Append("=").Append(value).Append(",");
            }
            sb.Append("}");
            return sb.ToString();
        }
        
        public static string ToCustomString<T>(IList<T> list)
        {
            var sb = new StringBuilder("[");
            foreach (var item in list)
            {
                sb.Append(item).Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}