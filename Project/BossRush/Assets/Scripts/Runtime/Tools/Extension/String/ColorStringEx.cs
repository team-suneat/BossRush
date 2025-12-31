using UnityEngine;

namespace TeamSuneat
{
    public static class ColorStringEx
    {
        public static string ToColorValueString(StringColorTypes type, string value)
        {
            return string.Format("<style={0}>{1}</style>", type, value.ToString());
        }

        public static string ToColorValueString<T>(this T value, StringColorTypes type)
        {
            return string.Format("<style={0}>{1}</style>", type, value.ToString());
        }

        public static string ToColorGradeString(this GradeNames grade, string value)
        {
            if (grade != GradeNames.None)
            {
                return string.Format("<style={0}>{1}</style>", grade, value.ToString());
            }

            return value;
        }

        public static string ToColorGradeStringGUI(this GradeNames grade, string value)
        {
            UnityEngine.Color gradeColor = grade.GetGradeColor();
            string hex = ColorEx.ColorToHex(gradeColor);
            return string.Format("<color={1}>{0}</color>", value, hex);
        }

        public static string ToColorString(this int value, string colorHex)
        {
            return ToColorString(value.ToString(), colorHex);
        }

        public static string ToColorString(this int value, Color color)
        {
            return ToColorString(value.ToString(), ColorEx.ColorToHex(color));
        }

        public static string ToColorString(this float value, Color color)
        {
            return ToColorString(value.ToString(), ColorEx.ColorToHex(color));
        }

        public static string ToColorString(this string value, Color color)
        {
            return ToColorString(value, ColorEx.ColorToHex(color));
        }

        public static string ToColorString(this string value, string hex)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return string.Format("<color={1}>{0}</color>", value.ToString(), hex);
        }

        public static string ToBoolFloatString(this float value)
        {
            if (value > 0)
            {
                return value.ToSelectString();
            }
            else if (value < 0)
            {
                return value.ToErrorString();
            }
            else
            {
                return value.ToString();
            }
        }

        public static string ToBoolString(this bool value)
        {
#if UNITY_EDITOR
            if (value)
            {
                return value.ToSelectString();
            }
            else
            {
                return value.ToErrorString();
            }
#else
            return value.ToString();
#endif
        }

        public static string ToValueString<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return ToColorString(value, GameColors.Value);
        }

        public static string ToValueString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return ToColorString(value.ToString(), GameColors.Value);
        }

        public static string ToSelectString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return ToColorString(value.ToString(), GameColors.Select);
        }

        public static string ToSelectString<T>(this T value, T defaultValue)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (defaultValue.Equals(value))
            {
                return value.ToString();
            }

            return ToColorString(value.ToString(), GameColors.Select);
        }

        public static string ToSelectString<T>(this T value, bool isEnable)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (isEnable)
            {
                return ToColorString(value.ToString(), GameColors.Select);
            }

            return value.ToString();
        }

        public static string ToErrorString<T>(this T value, T defaultValue)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (defaultValue.Equals(value))
            {
                return value.ToString();
            }

            return ToColorString(value.ToString(), GameColors.CherryRed);
        }

        public static string ToErrorString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return ToColorString(value.ToString(), GameColors.CherryRed);
        }

        public static string ToWarningString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return ToColorString(value.ToString(), GameColors.Yellow);
        }

        public static string ToDisableString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return ToColorString(value.ToString(), GameColors.DarkGray);
        }
    }
}