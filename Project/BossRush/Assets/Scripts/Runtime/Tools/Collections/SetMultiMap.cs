using System;
using System.Collections.Generic;

namespace TeamSuneat
{
    [Serializable]
    public class SetMultiMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, HashSet<TValue>> storage = new();

        public Dictionary<TKey, HashSet<TValue>> Storage => storage;

        public int KeysCount => storage.Keys.Count;

        public IEnumerable<TKey> Keys => storage.Keys;

        public int Count
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<TKey, HashSet<TValue>> kvp in storage)
                {
                    count += kvp.Value.Count;
                }
                return count;
            }
        }

        public bool Add(TKey key, TValue value)
        {
            if (!storage.TryGetValue(key, out HashSet<TValue> set))
            {
                set = new HashSet<TValue>();
                storage[key] = set;
            }
            return set.Add(value);
        }

        public bool Remove(TKey key, TValue value)
        {
            if (storage.TryGetValue(key, out HashSet<TValue> set) && set.Remove(value))
            {
                if (set.Count == 0)
                {
                    _ = storage.Remove(key);
                }

                return true;
            }
            return false;
        }

        public void Clear()
        {
            storage.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return storage.ContainsKey(key);
        }

        public bool Contains(TKey key, TValue value)
        {
            return storage.TryGetValue(key, out HashSet<TValue> set) && set.Contains(value);
        }

        public HashSet<TValue> this[TKey key]
        {
            get
            {
                if (!storage.ContainsKey(key))
                {
                    throw new KeyNotFoundException($"The given key {key} was not found in the collection.");
                }
                return storage[key];
            }
        }
    }
}