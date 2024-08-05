
using System.Collections;

namespace Evil.Util
{
    /// <summary>
    /// LinkedHashMap is a dictionary which preserve the order of inserting elements.
    /// It's similar to LinkedHashMap in Java.
    /// Could be used for LRU caches.
    /// </summary>
    public class LinkedHashMap<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        public int Count => m_ValueByKey.Count;
        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => m_ValueByKey.Keys;
        public ICollection<TValue> Values => m_ValueByKey.Values.Select(pair => pair.Value).ToList();

        private LinkedList<TKey> m_Items;
        private IDictionary<TKey, ValueNodePair> m_ValueByKey;

        public void Clear()
        {
            m_Items = new LinkedList<TKey>();
            m_ValueByKey = new Dictionary<TKey, ValueNodePair>();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return m_ValueByKey.TryGetValue(item.Key, out var value) && value.Value!.Equals(item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return m_ValueByKey.ContainsKey(key);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (ContainsKey(key)) throw new ArgumentException("Key is already presented", nameof(key));
            var node = m_Items.AddLast(key);
            m_ValueByKey.Add(key, new ValueNodePair(value, node));
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!m_ValueByKey.TryGetValue(key, out var tempValue)) return false;
            var node = tempValue.Node;
            m_ValueByKey.Remove(key);
            m_Items.Remove(node);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!m_ValueByKey.TryGetValue(key, out var tempValue)) {
                value = default;
                return false;
            }

            value = tempValue.Value;
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var maxLen = Math.Max(array.Length, arrayIndex + Count);
            using var enumerator = GetEnumerator();
            for (var i = arrayIndex; i < maxLen; i++) {
                array[i] = enumerator.Current;
                enumerator.MoveNext();
            }
        }

        public TValue this[TKey key] {
            get => m_ValueByKey[key].Value;
            set {
                if (!m_ValueByKey.ContainsKey(key)) {
                    Add(key, value);
                }
                else {
                    m_ValueByKey[key].Value = value;
                    UpdateKey(key);
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var node = m_Items.First;
            while (node != null) {
                yield return KeyValuePair.Create(node.Value, m_ValueByKey[node.Value].Value);
                node = node.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UpdateKey(TKey key)
        {
            if (!m_ValueByKey.ContainsKey(key)) return;
            var node = m_ValueByKey[key].Node;
            if (node.Next == null) return;

            m_Items.Remove(node);
            m_Items.AddLast(node);
        }

        private void Init()
        {
            Clear();
        }

        private class ValueNodePair
        {
            public TValue Value { get; set; }
            public LinkedListNode<TKey> Node { get; }

            public ValueNodePair(TValue value, LinkedListNode<TKey> node)
            {
                Value = value;
                Node = node;
            }
        }

        public LinkedHashMap()
        {
            Init();
        }
    }
}