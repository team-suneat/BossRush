using System;
using System.Globalization;
using UnityEngine;

namespace TeamSuneat
{
    public static class DataConverter
    {
        public static TEnum ToEnum<TEnum>(string value, bool ignoreCase = true) where TEnum : struct
        {
            if (Enum.TryParse(value, ignoreCase, out TEnum result))
            {
                return result;
            }

            Log.Error("string을 Enum문으로 변환하는 데 실패하였습니다. Value: {0}, Type: {1}", value, typeof(TEnum));

            return default(TEnum);
        }

        public static TEnum[] ToEnumArray<TEnum>(string content, TEnum[] defaultValue) where TEnum : struct
        {
            if (string.IsNullOrEmpty(content))
            {
                return defaultValue;
            }
            if ("-" == content)
            {
                return defaultValue;
            }

            string[] values = content.Split(new char[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            TEnum[] result = new TEnum[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                result[i] = ToEnum<TEnum>(values[i]);
            }

            return result;
        }

        public static string[] ToStringArray(string content, string[] defaultValue)
        {
            if (string.IsNullOrEmpty(content))
            {
                return defaultValue;
            }

            if ("-" == content)
            {
                return defaultValue;
            }

            return content.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int[] ToIntArray(string content, int[] defaultValue)
        {
            if (string.IsNullOrEmpty(content))
            {
                return defaultValue;
            }

            if ("-" == content)
            {
                return defaultValue;
            }

            string[] values = content.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                result[i] = int.Parse(values[i]);
            }

            return result;
        }

        public static float[] ToFloatArray(string content, float[] defaultValue)
        {
            if (string.IsNullOrEmpty(content))
            {
                return defaultValue;
            }

            if ("-" == content)
            {
                return defaultValue;
            }

            string[] values = content.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            float[] result = new float[values.Length];

            CultureInfo culture = CultureInfo.InvariantCulture; // 점을 소수점 구분자로 사용하는 문화권 지정

            for (int i = 0; i < values.Length; i++)
            {
                float value = float.Parse(values[i], culture);
                result[i] = MathF.Round(value, 4);
            }

            return result;
        }

        public static Vector2 ToVector2(string content, Vector2 defaultValue)
        {
            if (string.IsNullOrEmpty(content))
            {
                return defaultValue;
            }

            if ("-" == content)
            {
                return defaultValue;
            }

            string[] values = content.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2)
            {
                return defaultValue;
            }

            float[] result = new float[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                result[i] = float.Parse(values[i]);
            }

            return new Vector2(result[0], result[1]);
        }
    }
}