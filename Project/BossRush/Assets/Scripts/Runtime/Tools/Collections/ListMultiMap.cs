using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamSuneat
{
    [Serializable]
    public class ListMultiMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> storage = new();

        public Dictionary<TKey, List<TValue>> Storage => storage;

        public TKey[] Keys => storage.Keys.ToArray();

        public int KeysCount => storage.Keys.Count;

        public int Count
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<TKey, List<TValue>> kvp in storage)
                {
                    count += kvp.Value.Count;
                }
                return count;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (!storage.TryGetValue(key, out List<TValue> list))
            {
                list = new List<TValue>();
                storage[key] = list;
            }
            list.Add(value);
        }

        public void Remove(TKey key, TValue value)
        {
            if (storage.TryGetValue(key, out List<TValue> list) && list.Contains(value))
            {
                _ = list.Remove(value);
                if (list.Count == 0)
                {
                    _ = storage.Remove(key);
                }
            }
        }

        public void RemoveAll(TKey key)
        {
            if (storage.ContainsKey(key))
            {
                _ = storage.Remove(key);
            }
        }

        public void Clear()
        {
            storage.Clear();
        }

        public int ClearNull()
        {
            List<TKey> keysToRemove = new();
            foreach (KeyValuePair<TKey, List<TValue>> kvp in storage)
            {
                List<TValue> valueList = kvp.Value;
                _ = valueList.RemoveAll(value => value == null);
                if (valueList.Count == 0)
                {
                    keysToRemove.Add(kvp.Key);
                    Log.Progress("연결이 끊긴 Value를 모두 삭제합니다: {0}", kvp.Key.ToSelectString());
                }
            }

            foreach (TKey key in keysToRemove)
            {
                _ = storage.Remove(key);
            }

            return keysToRemove.Count;
        }

        //

        public bool Contains(TKey key, TValue value)
        {
            return storage.TryGetValue(key, out List<TValue> list) && list.Contains(value);
        }

        public bool ContainsKey(TKey key)
        {
            return storage.ContainsKey(key);
        }

        public bool ContainsValue(TKey key)
        {
            return storage.TryGetValue(key, out List<TValue> list) && list.Count > 0;
        }

        //

        public TValue FindFirst(TKey key)
        {
            if (storage.TryGetValue(key, out List<TValue> list) && list.Count > 0)
            {
                return list[0];
            }

            return default;
        }

        //

        public int GetValueCount(TKey key)
        {
            return storage.TryGetValue(key, out List<TValue> list) ? list.Count : 0;
        }

        public bool TryGetValue(TKey key, out List<TValue> values)
        {
            if (storage.TryGetValue(key, out values))
            {
                return true;
            }
            // Log.Warning($"해당 키({key}, {typeof(TKey)})를 가진 값({typeof(TValue)})을 찾을 수 없습니다.");
            values = null;
            return false;
        }
    }
}