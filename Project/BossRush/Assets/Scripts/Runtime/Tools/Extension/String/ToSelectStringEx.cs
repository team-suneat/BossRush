using System;

namespace TeamSuneat
{
    public static class ToSelectStringEx
    {
        /// <summary>
        /// int 값을 ToSelectString으로 변환합니다.
        /// </summary>
        public static string ToSelectString(this int value)
        {
            return value.ToString();
        }
    }
}
