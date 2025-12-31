using System;
using System.Text;
using System.Text.RegularExpressions;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetStringKey(this CharacterGrowthTypes key)
        {
            return $"Growth_Type_{key}";
        }

        public static string GetLocalizedString(this CharacterGrowthTypes key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this CharacterGrowthTypes key, LanguageNames languageName)
        {
            string result = JsonDataManager.FindStringClone(key.GetStringKey(), languageName);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return key.ToString();
        }

        public static string GetStringKey(this StatNames key)
        {
            return $"Stat_Name_{key}";
        }

        public static string GetLocalizedString(this StatNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this StatNames key, LanguageNames languageName)
        {
            string result = JsonDataManager.FindStringClone(key.GetStringKey(), languageName);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return key.ToString();
        }

        public static string ReplaceStatValue(string input, bool useColor = true)
        {
            string pattern = @"\{\$Stat\.([a-zA-Z0-9_]+)\}";

            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string statName = match.Groups[1].Value;
                    StatNames enumStatName;
                    if (Enum.TryParse(statName, out enumStatName))
                    {
                        if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null)
                        {
                            float statValue = CharacterManager.Instance.Player.Stat.FindValueOrDefault(enumStatName);
                            string replacement = enumStatName.GetStatValueString(statValue, useColor);

                            input = input.Replace(match.Value, replacement);
                        }
                        else
                        {
                            input = input.Replace(match.Value, "0");
                        }
                    }
                }
            }

            return input;
        }

        public static string ReplaceStatValue(string input, float multiplier, bool useColor = true)
        {
            string pattern = @"\{\$Stat\.([a-zA-Z0-9_]+)\}";

            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string statName = match.Groups[1].Value;
                    StatNames enumStatName;
                    if (Enum.TryParse(statName, out enumStatName))
                    {
                        if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null)
                        {
                            float statValue = CharacterManager.Instance.Player.Stat.FindValueOrDefault(enumStatName);
                            string replacement;
                            if (multiplier.IsEqual(1f))
                            {
                                replacement = enumStatName.GetStatValueString(statValue, useColor);
                            }
                            else
                            {
                                replacement = $"{enumStatName.GetStatValueString(statValue, useColor)}({enumStatName.GetStatValueString(statValue * multiplier, useColor)})";
                            }

                            input = input.Replace(match.Value, replacement);
                        }
                        else
                        {
                            input = input.Replace(match.Value, "0");
                        }
                    }
                }
            }

            return input;
        }
    }
}