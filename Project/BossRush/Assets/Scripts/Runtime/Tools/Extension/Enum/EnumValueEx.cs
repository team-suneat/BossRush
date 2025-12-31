using System;
using System.Text;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat

{
    public static class EnumValueEx
    {
        #region Stat String

        public static string GetStatValueString(this StatNames statName, float statValue)
        {
            StatData statData = JsonDataManager.FindStatData(statName);
            if (statData.IsValid())
            {
                if (statName.IsPercent())
                {
                    return ValueStringEx.GetPercentStringWithDigit(statValue, statData.Digit);
                }
                else
                {
                    return ValueStringEx.GetFormattedString(statValue, statData.Digit, false, null);
                }
            }
            Log.Warning("능력치 데이터를 찾을 수 없습니다. {0}({1})", statName, statName.ToLogString());
            return "0";
        }

        public static string GetStatValueString(this StatNames statName, float statValue, bool useColor)
        {
            StatData statData = JsonDataManager.FindStatData(statName);
            bool colorFlag = useColor;

            if (statName.IsPercent())
            {
                return ValueStringEx.GetPercentStringWithDigit(statValue, statData.Digit, colorFlag);
            }
            else
            {
                return ValueStringEx.GetFormattedString(statValue, statData.Digit, colorFlag, null);
            }
        }

        public static string GetStatValueString(this StatNames statName, float statValue, float defaultValue)
        {
            StatData statData = JsonDataManager.FindStatData(statName);
            bool colorFlag = !float.IsNaN(defaultValue) && statValue != defaultValue;

            if (statName.IsPercent())
            {
                return ValueStringEx.GetPercentStringWithDigit(statValue, statData.Digit, colorFlag);
            }
            else
            {
                return ValueStringEx.GetFormattedString(statValue, statData.Digit, colorFlag, null);
            }
        }

        public static string GetStatValueString(this StatNames statName, float statValue, Color color)
        {
            StatData statData = JsonDataManager.FindStatData(statName);
            if (statName.IsPercent())
            {
                return ValueStringEx.GetPercentStringWithDigit(statValue, statData.Digit, color);
            }
            else
            {
                return ValueStringEx.GetFormattedString(statValue, statData.Digit, false, color);
            }
        }

        #endregion Stat String

        #region Range String

        /// <summary> 주어진 최소값과 최대값을 포맷팅하여 문자열로 변환합니다. </summary>
        public static string GetRangeString(this StatNames statName, float minValue, float maxValue)
        {
            StringBuilder sb = new StringBuilder();
            StatData statData = JsonDataManager.FindStatData(statName);

            float formattedMin = FormatStatValue(minValue, statData);
            float formattedMax = FormatStatValue(maxValue, statData);

            if (!statName.IsReduceStat())
            {
                sb.Append("+");
            }

            string rangeFormat = GameSetting.Instance.Language.Name == LanguageNames.Korean ? "({0}~{1})" : "({0}-{1})";
            sb.AppendFormat(rangeFormat, formattedMin, formattedMax);

            if (statName.IsPercent())
            {
                sb.Append("%");
            }

            return sb.ToString();
        }

        #endregion Range String

        /// <summary> StatData에 맞게 값을 포맷팅합니다. </summary>
        private static float FormatStatValue(float value, StatData statData)
        {
            if (statData.Mod is StatModType.PercentAdd or StatModType.PercentMulti)
            {
                return MathF.Abs(MathF.Round(value * 100, statData.Digit));
            }
            else
            {
                return MathF.Abs(MathF.Round(value, statData.Digit));
            }
        }
    }
}