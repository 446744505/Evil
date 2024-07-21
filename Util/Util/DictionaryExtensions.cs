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
    }
}