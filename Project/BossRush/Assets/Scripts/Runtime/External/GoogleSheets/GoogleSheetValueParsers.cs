using System;

namespace TeamSuneat
{
    /// <summary>
    /// 구글 시트 문자열 값을 안전하게 형변환하는 공용 유틸리티.
    /// - 규칙: null/빈 문자열/"-" 는 기본값으로 처리합니다.
    /// - 배열 구분자: 콤마(,)
    /// - 구현 전략: 가능한 경우 기존 TeamSuneat.DataConverter를 위임 호출하여 중복 로직을 줄입니다.
    /// </summary>
    public static class GoogleSheetValueParsers
    {
        public static bool TryParseInt(string text, out int value, int defaultValue = 0)
        {
            if (IsNullOrDash(text)) { value = defaultValue; return true; }
            return int.TryParse(text, out value);
        }

        public static bool TryParseFloat(string text, out float value, float defaultValue = 0)
        {
            if (IsNullOrDash(text)) { value = defaultValue; return true; }
            return float.TryParse(text, out value);
        }

        public static bool TryParseBool(string text, out bool value, bool defaultValue = false)
        {
            if (IsNullOrDash(text)) { value = defaultValue; return true; }
            // true/false, 1/0 등 확장 가능
            if (bool.TryParse(text, out value))
            {
                return true;
            }

            if (text == "1") { value = true; return true; }
            if (text == "0") { value = false; return true; }
            value = defaultValue; return false;
        }

        public static bool TryParseEnum<TEnum>(string text, out TEnum value, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            if (IsNullOrDash(text)) { value = defaultValue; return true; }
            return Enum.TryParse(text, ignoreCase: true, out value);
        }

        public static bool TryParseEnumArray<TEnum>(string text, out TEnum[] values, char delimiter = ',') where TEnum : struct, Enum
        {
            if (IsNullOrDash(text)) { values = Array.Empty<TEnum>(); return true; }
            // DataConverter는 콤마/공백 등 복수 구분자를 허용. 프로젝트 전역 규칙과 통일을 위해 위임 사용.
            values = DataConverter.ToEnumArray(text, Array.Empty<TEnum>());
            return true;
        }

        public static bool TryParseIntArray(string text, out int[] values, char delimiter = ',')
        {
            if (IsNullOrDash(text)) { values = Array.Empty<int>(); return true; }
            values = DataConverter.ToIntArray(text, Array.Empty<int>());
            return true;
        }

        public static bool TryParseFloatArray(string text, out float[] values, char delimiter = ',')
        {
            if (IsNullOrDash(text)) { values = Array.Empty<float>(); return true; }
            values = DataConverter.ToFloatArray(text, Array.Empty<float>());
            return true;
        }

        private static bool IsNullOrDash(string text)
        {
            return string.IsNullOrEmpty(text) || text == "-";
        }
    }
}