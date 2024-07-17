namespace Evil.Util
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 删除指定键并返回对应的值
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static bool RemoveAndReturnValue<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, out TValue? value) where TKey : notnull
        {
            if (dict.TryGetValue(key, out value))
            {
                dict.Remove(key);
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool PutAndReturnValue<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, TValue value, out TValue? old) where TKey : notnull
        {
            var hadOld = dict.TryGetValue(key, out old);
            dict[key] = value;
            return hadOld;
        }
    }
}