using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamSuneat
{
    public static class DataValidator
    {
        public static bool IsValid<T>(this T[] array, int index)
        {
            return array != null && array.Length > index && index >= 0;
        }

        public static bool IsValid<T>(this T[] array)
        {
            return array != null && array.Length > 0;
        }

        //

        public static bool IsValidArray(this int[] array)
        {
            if (array == null)
            {
                return false;
            }

            if (array.Length == 0)
            {
                return false;
            }

            if (array.Length == 1 && array[0] == 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsValidArray(this float[] array)
        {
            if (array == null)
            {
                return false;
            }

            if (array.Length == 0)
            {
                return false;
            }

            if (array.Length == 1 && array[0].IsZero())
            {
                return false;
            }

            return true;
        }

        public static bool IsValidArray(this string[] array)
        {
            if (array == null)
            {
                return false;
            }

            if (array.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (!string.IsNullOrEmpty(array[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidArray<T>(this T[] array) where T : Enum
        {
            if (array == null)
            {
                return false;
            }

            if (array.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].Equals(default(T)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidArray(this Component[] array)
        {
            if (array == null)
            {
                return false;
            }

            if (array.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidArray(this GameObject[] array)
        {
            if (array == null)
            {
                return false;
            }

            if (array.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidArray<T>(this IList<T>[] list)
        {
            if (list == null)
            {
                return false;
            }

            if (list.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < list.Length; i++)
            {
                if (!list[i].Equals(default(T)))
                {
                    return true;
                }
            }

            return false;
        }

        //

        public static bool IsValid<TValue>(this List<TValue> list)
        {
            return list != null && list.Count > 0;
        }

        public static bool IsValid<TValue>(this List<TValue> list, int index)
        {
            return list != null && list.Count > index && index >= 0;
        }

        public static bool IsValid<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary != null && dictionary.Count > 0;
        }

        public static bool IsValid<TKey, TValue>(this ListMultiMap<TKey, TValue> multiMap)
        {
            return multiMap != null && multiMap.Count > 0;
        }

        //

        public static bool IsValid(this IData<int> jsonData)
        {
            if (jsonData == null)
            {
                return false;
            }
            if (jsonData.GetKey() == 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsValid(this IData<string> jsonData)
        {
            if (jsonData == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(jsonData.GetKey()))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidArray(this IData<int>[] jsonData)
        {
            if (jsonData == null)
            {
                return false;
            }

            if (jsonData.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < jsonData.Length; i++)
            {
                if (jsonData[i].IsValid())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidArray(this IData<string>[] jsonData)
        {
            if (jsonData == null)
            {
                return false;
            }

            if (jsonData.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < jsonData.Length; i++)
            {
                if (jsonData[i].IsValid())
                {
                    return true;
                }
            }

            return false;
        }

        //

        public static void RemoveNull<TValue>(this List<TValue> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                }
            }
        }

        public static TValue[] RemoveNull<TValue>(this TValue[] array)
        {
            if (array != null && array.Length > 0)
            {
                List<TValue> list = new List<TValue>(array);

                list.RemoveNull();

                return list.ToArray();
            }

            return null;
        }
    }
}