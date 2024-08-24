using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Evil.Util
{
    public interface ILruCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        bool Contains(TKey key);
        bool TryGet(TKey key, [MaybeNullWhen(false)]out TValue value);
        TValue? Lookup(TKey key);
        void Add(TKey key, TValue value);
        void Clear();
        bool Remove(TKey key);
    }

    public class LruCache<TKey, TValue> : ILruCache<TKey, TValue> where TKey : notnull
    {
        private readonly int m_Capacity;
        private readonly Action? m_OnLruRemove;
        private readonly LinkedHashMap<TKey, TValue> m_LinkedHashMap = new();
        
        public int Count => m_LinkedHashMap.Count;
        public ICollection<TValue> Values => m_LinkedHashMap.Values;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_LinkedHashMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(TKey key)
        {
            return m_LinkedHashMap.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return m_LinkedHashMap.Remove(key);
        }

        public bool TryGet(TKey key, [MaybeNullWhen(false)]out TValue value)
        {
            var result = m_LinkedHashMap.TryGetValue(key, out value);
            m_LinkedHashMap.UpdateKey(key);
            return result;
        }

        public TValue? Lookup(TKey key)
        {
            return !TryGet(key, out var value) ? default : value;
        }

        public void Add(TKey key, TValue value)
        {
            if (m_LinkedHashMap.Count == m_Capacity) {
                m_LinkedHashMap.Remove(m_LinkedHashMap.First().Key);
                m_OnLruRemove?.Invoke();
            }
            m_LinkedHashMap.Add(key, value);
        }

        public void Clear()
        {
            m_LinkedHashMap.Clear();
        }

        public LruCache(int capacity, Action? onLruRemove = null)
        {
            m_Capacity = capacity;
            m_OnLruRemove = onLruRemove;
        }
    }
}