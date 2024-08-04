using System.Collections;
using System.Text;

namespace Evil.Util
{
    public class Strings
    {
        public static string ToCustomString<TKey, TValue>(IDictionary<TKey, TValue>? dict)
        {
            if (dict == null)
            {
                return "null";
            }
            var sb = new StringBuilder("{");
            foreach (var (key, value) in dict)
            {
                sb.Append(key).Append("=").Append(value).Append(",");
            }
            sb.Append("}");
            return sb.ToString();
        }
        
        public static string ToCustomStringN(IDictionary? dict)
        {
            if (dict == null)
            {
                return "null";
            }
            var sb = new StringBuilder("{");
            foreach (var pair in dict)
            {
                var entry = (DictionaryEntry) pair;
                sb.Append(entry.Key).Append("=").Append(entry.Value).Append(",");
            }
            sb.Append("}");
            return sb.ToString();
        }
        
        public static string ToCustomString<T>(IList<T>? list)
        {
            if (list == null)
            {
                return "null";
            }
            var sb = new StringBuilder("[");
            foreach (var item in list)
            {
                sb.Append(item).Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
        
        public static string ToCustomStringN(IList? list)
        {
            if (list == null)
            {
                return "null";
            }
            var sb = new StringBuilder("[");
            foreach (var item in list)
            {
                sb.Append(item).Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
        
        public static string ToCustomString<T>(ISet<T>? set)
        {
            if (set == null)
            {
                return "null";
            }
            var sb = new StringBuilder("[");
            foreach (var item in set)
            {
                sb.Append(item).Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}