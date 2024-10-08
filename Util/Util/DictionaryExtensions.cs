
namespace Evil.Util
{
    public static class DictionaryExtensions
    {
        public static bool PutAndReturnValue<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, TValue value, out TValue? old) where TKey : notnull
        {
            var hadOld = dict.TryGetValue(key, out old);
            dict[key] = value;
            return hadOld;
        }
        
        public static void PutAll<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other) where TKey : notnull
        {
            foreach (var (key, value) in other)
            {
                dict[key] = value;
            }
        }
        
        public static TValue ComputeIfAbsent<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> f) where TKey : notnull
        {
            if (dict.TryGetValue(key, out var value))
                return value;
            value = f(key);
            dict[key] = value;
            return value;
        }
        
        public static TValue? ComputeIfPresent<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue, TValue> f) where TKey : notnull
        {
            if (!dict.TryGetValue(key, out var value))
                return default;
            value = f(key, value);
            dict[key] = value;
            return value;
        }
    }
}