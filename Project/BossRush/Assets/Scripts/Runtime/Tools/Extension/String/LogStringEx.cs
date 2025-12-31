using System;

namespace TeamSuneat
{
    public static class LogStringEx
    {
        /// <summary>
        /// 로그용 문자열로 변환합니다.
        /// </summary>
        public static string ToLogString<T>(this T value) where T : Enum
        {
            return value.ToString();
        }

        public static string ToLogString(this string value)
        {
            return value.ToString();
        }
    }
}