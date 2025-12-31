using System.Text.RegularExpressions;

namespace TeamSuneat
{
    public enum StringStyleType
    {
        None,
        UI,
        GUI,
    }

    public static partial class StringGetter
    {
        public static string AddStyleString(this string input, string style)
        {
            return string.Format("<style={1}>{0}</style>", input, style);
        }

        public static string RemoveStyleString(this string input)
        {
            string pattern = @"<style=[^>]*>(.*?)<\/style>";
            string replacement = "$1";

            return Regex.Replace(input, pattern, replacement);
        }

        public static string ConvertStyleToColorString(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string output = input;

                output = ConvertStyleFormatString("SkillName", @"<b><color=white>$1</color></b>", output);
                output = ConvertStyleFormatString("BuffName", @"<i><color=white>$1</color></i>", output);
                output = ConvertStyleFormatString("StateEffect", @"<i><color=white>$1</color></i>", output);
                if (output.Contains("StateEffect"))
                {
                    UnityEngine.Debug.LogWarningFormat("상태이상 스타일이 컬러 적용에 실패했습니다: {0}, {1}", input, output);
                }

                output = ConvertStyleFormatString("StatName", @"<color=white>$1</color>", output);
                output = ConvertStyleFormatString("Synergy", @"<color=white>$1</color>", output);

                output = ConvertStyleFormatString("Area", @"<color=#E5CC98>$1</color>", output);
                output = ConvertStyleFormatString("Stage", @"<color=#E5CC98>$1</color>", output);

                output = ConvertStyleFormatString("Select", @"<color=#7CFC00>$1</color>", output);
                output = ConvertStyleFormatString("Value", @"<color=#D59F36>$1</color>", output);
                output = ConvertStyleFormatString("RangeValue", @"<color=#007683>$1</color>", output);
                output = ConvertStyleFormatString("Range", @"<color=#007683>$1</color>", output);

                output = ConvertStyleFormatString("Character", @"<color=#E5CC98>$1</color>", output);
                output = ConvertStyleFormatString("NPC", @"<color=#F89301>$1</color>", output);
                output = ConvertStyleFormatString("Monster", @"<color=#9E1C10>$1</color>", output);

                // Currency styles
                output = ConvertStyleFormatString("Gold", @"<color=#FFD700>$1</color>", output);
                output = ConvertStyleFormatString("MagicStone", @"<color=#E2005B>$1</color>", output);
                output = ConvertStyleFormatString("RubyKey", @"<color=#FE1A00>$1</color>", output);
                output = ConvertStyleFormatString("SoulShard", @"<color=#9C102C>$1</color>", output);
                output = ConvertStyleFormatString("OblivionCrystal", @"<color=#7302DD>$1</color>", output);
                output = ConvertStyleFormatString("RiftShard", @"<color=#FF8C00>$1</color>", output);

                return output;
            }

            return input;
        }

        private static string ConvertStyleFormatString(string styleName, string replaceFormat, string input)
        {
            // 정규 표현식을 사용하여 <style=styleName> 및 </style> 부분을 찾습니다.
            string pattern = string.Format(@"<style={0}>(.*?)</style>", styleName);

            // 대응되는 부분을 찾아내어 변경합니다.
            string result = Regex.Replace(input, pattern, replaceFormat);

            return result;
        }

        public static string RemoveStyleTag(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. <style=...>태그 제거
            string result = Regex.Replace(input, @"<style=.*?>", string.Empty, RegexOptions.IgnoreCase);

            // 2. </style> 태그 제거
            result = Regex.Replace(result, @"</style>", string.Empty, RegexOptions.IgnoreCase);

            return result;
        }

        public static string RemoveColorTag(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. <color=...>태그 제거
            string result = Regex.Replace(input, @"<color=.*?>", string.Empty, RegexOptions.IgnoreCase);

            // 2. </color> 태그 제거
            result = Regex.Replace(result, @"</color>", string.Empty, RegexOptions.IgnoreCase);

            return result;
        }

        public static string RemoveStyleColorTag(this string input)
        {
            string output = RemoveStyleTag(input);
            return RemoveColorTag(output);
        }

        public static string RemoveBoldTag(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // <b>와 </b> 태그 제거
            string result = Regex.Replace(input, @"<b>", string.Empty, RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"</b>", string.Empty, RegexOptions.IgnoreCase);

            return result;
        }

        public static string RemoveItalicTag(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // <i>와 </i> 태그 제거
            string result = Regex.Replace(input, @"<i>", string.Empty, RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"</i>", string.Empty, RegexOptions.IgnoreCase);

            return result;
        }
    }
}