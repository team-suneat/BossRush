using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetNameString(this PassiveNames key)
        {
            return GetNameString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetNameString(this PassiveNames key, LanguageNames languageName)
        {
            if (key == PassiveNames.None)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Passive_Name_");
            stringBuilder.Append(key.ToString());

            string result = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);

            if (string.IsNullOrEmpty(result))
            {
                Log.Warning($"패시브 이름({key})을 찾을 수 없습니다.");
                return key.ToString();
            }

            return result;
        }

        //

        public static string GetDescString(this PassiveNames key)
        {
            return GetDescString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetDescString(this PassiveNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Passive_Desc_");
            stringBuilder.Append(key.ToString());

            string content = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);
            if (!string.IsNullOrEmpty(content))
            {
                content = ReplaceStatName(content);
            }
            return content;
        }

        public static string ReplacePassiveValue(string input, int level, bool useColor, bool isShowNextLevel)
        {
            string output;

            output = ReplacePassiveValueWithTwoGroups(input, level, useColor, isShowNextLevel);
            output = ReplacePassiveValueWithFourGroups(output, level, useColor, isShowNextLevel);

            return output;
        }

        private static string ReplacePassiveValueWithTwoGroups(string input, int level, bool useColor, bool isShowNextLevel)
        {
            /// [Passive.PassiveName.FieldName]
            /// 예시: [Passive.SwordOfFire.TriggerChance]
            string pattern = @"\[Passive\.([a-zA-Z0-9_]+)\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string passiveName = match.Groups[1].Value;
                    string fieldName = match.Groups[2].Value;

                    if (!Enum.TryParse(passiveName, out PassiveNames enumPassiveName))
                    {
                        Log.Error("패시브의 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    PassiveAsset passiveAsset = ScriptableDataManager.Instance.FindPassive(enumPassiveName);
                    if (!passiveAsset.IsValid())
                    {
                        Log.Error("패시브의 에셋을 찾을 수 없습니다. {0}", enumPassiveName.ToLogString());
                        continue;
                    }

                    if (fieldName.Contains("Trigger"))
                    {
                        if (fieldName.Contains("Count") || fieldName.Contains("StatValue") || fieldName.Contains("ResourceValue"))
                        {
                            string intValue = LoadIntValueString(fieldName, passiveAsset.TriggerSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel);
                            input = input.Replace(match.Value, intValue);
                        }
                        else if (fieldName.Contains("Chance") || fieldName.Contains("Percent"))
                        {
                            string percentValue = LoadPercentString(fieldName, passiveAsset.TriggerSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel);
                            input = input.Replace(match.Value, percentValue);
                        }
                        else
                        {
                            Log.Error("패시브의 변수를 찾을 수 없습니다. {0}", fieldName);
                        }
                    }
                    else if (fieldName.Contains("Condition"))
                    {
                        if (fieldName.Contains("ResourceValue"))
                        {
                            string intValue = LoadIntValueString(fieldName, passiveAsset.ConditionSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel);
                            input = input.Replace(match.Value, intValue);
                        }
                        else if (fieldName.Contains("Ratio"))
                        {
                            string percentValue = LoadPercentString(fieldName, passiveAsset.ConditionSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel);
                            input = input.Replace(match.Value, percentValue);
                        }
                        else
                        {
                            Log.Error("패시브의 변수를 찾을 수 없습니다. {0}", fieldName);
                        }
                    }
                    else
                    {
                        if (fieldName.Contains("ApplyMaxCount") || fieldName.Contains("RewardLevel"))
                        {
                            input = input.Replace(match.Value, LoadIntValueString(fieldName, passiveAsset.EffectSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel));
                        }
                        else if (fieldName.Contains("Rest"))
                        {
                            input = input.Replace(match.Value, LoadFloatValueString(fieldName, passiveAsset.EffectSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel));
                        }
                        else if (fieldName.Contains("Duration"))
                        {
                            input = input.Replace(match.Value, LoadFloatValueString(fieldName, passiveAsset.EffectSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel));
                        }
                        else if (fieldName.Contains("Apply") || fieldName.Contains("Add"))
                        {
                            input = input.Replace(match.Value, LoadPercentString(fieldName, passiveAsset.EffectSettings, level, passiveAsset.MaxLevel, useColor, isShowNextLevel));
                        }
                        else
                        {
                            Log.Error("패시브의 변수를 찾을 수 없습니다. {0}", fieldName);
                        }
                    }
                }
            }

            return input;
        }

        private static string ReplacePassiveValueWithFourGroups(string input, int level, bool useColor, bool isShowNextLevel)
        {
            /// [Passive.PassiveName.FieldName[FieldIndex].FieldName2]]
            /// 예시: [Passive.SwiftCruelStrike.ReduceSkillCooldowns[0].ReduceCooldownTime]
            string pattern = @"\[Passive\.([a-zA-Z0-9_*]+)\.([a-zA-Z0-9_*]+)\[([0-9_*]+)\]\.([a-zA-Z0-9_*]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string nameString = match.Groups[1].Value;
                    string fieldNameString = match.Groups[2].Value;
                    string fieldNumberString = match.Groups[3].Value;
                    string fieldName2String = match.Groups[4].Value;

                    PassiveNames passiveName = EnumEx.ConvertTo<PassiveNames>(nameString);
                    if (passiveName == PassiveNames.None)
                    {
                        Log.Error("패시브의 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    PassiveEffectSettings effectSettings = ScriptableDataManager.Instance.FindPassiveEffect(passiveName);
                    if (!effectSettings.IsValid())
                    {
                        Log.Error("패시브의 효과 에셋을 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    FieldInfo fieldInfo = typeof(PassiveEffectSettings).GetField(fieldNameString);
                    if (fieldInfo == null)
                    {
                        Log.Error("패시브의 변수를 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    if (!int.TryParse(fieldNumberString, out int fieldsIndex))
                    {
                        Log.Error("변수의 인덱스를 검사할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    object fieldObject = fieldInfo.GetValue(effectSettings);
                    string replacement = string.Empty;

                    if (fieldNameString.Contains("CurrencyRewards"))
                    {
                        PassiveRewardCurrency[] datas = (PassiveRewardCurrency[])fieldObject;
                        replacement = GetRewardCurrencyValues(datas[fieldsIndex], fieldName2String);
                    }
                    else
                    {
                        replacement = fieldObject.ToString();
                    }

                    input = input.Replace(match.Value, replacement);
                }
            }

            return input;
        }

        private static string LoadPercentString<T>(string fieldName, T passiveSettings, int level, int maxLevel, bool useColor, bool isShowNextLevel)
        {
            try
            {
                FieldInfo fieldInfo = typeof(T).GetField(fieldName);
                if (fieldInfo == null)
                {
                    Log.Error("패시브의 변수를 찾을 수 없습니다. {0}", fieldName);
                    return string.Empty;
                }

                string replacement;
                float value = (float)fieldInfo.GetValue(passiveSettings);
                if (maxLevel > 1)
                {
                    FieldInfo fieldInfoByLevel = typeof(T).GetField(fieldInfo.Name + "ByLevel");
                    float valueByLevel = (float)fieldInfoByLevel.GetValue(passiveSettings);
                    if (!valueByLevel.IsZero())
                    {
                        if (isShowNextLevel)
                        {
                            float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            float nextValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                            return string.Format("{0} ▶ {1}", ValueStringEx.GetPercentString(totalValue), ValueStringEx.GetPercentString(nextValue, true));
                        }
                        else
                        {
                            float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            return ValueStringEx.GetPercentString(totalValue, true);
                        }
                    }
                }

                replacement = ValueStringEx.GetPercentString((float)value);
                if (useColor)
                {
                    replacement = replacement.ToValueString();
                }

                return replacement;
            }
            catch (Exception e)
            {
                Log.Error($"[{fieldName}] from [{typeof(T)}]: {e.Message}");
                return string.Empty;
            }
        }

        private static string LoadIntValueString<T>(string fieldName, T passiveSettings, int level, int maxLevel, bool useColor, bool isShowNextLevel)
        {
            try
            {
                FieldInfo fieldInfo = typeof(T).GetField(fieldName);
                if (fieldInfo == null)
                {
                    Log.Error("패시브의 변수를 찾을 수 없습니다. {0}", fieldName);
                    return string.Empty;
                }

                string replacement;
                int value = (int)fieldInfo.GetValue(passiveSettings);
                if (maxLevel > 1)
                {
                    FieldInfo fieldInfoByLevel = typeof(T).GetField(fieldInfo.Name + "ByLevel");
                    int valueByLevel = (int)fieldInfoByLevel.GetValue(passiveSettings);
                    if (valueByLevel != 0)
                    {
                        if (isShowNextLevel)
                        {
                            int totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            int nextValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                            return string.Format("{0} ▶ {1}", ValueStringEx.GetValueString(totalValue), ValueStringEx.GetValueString(nextValue, true));
                        }
                        else
                        {
                            int totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            return ValueStringEx.GetValueString(totalValue, true);
                        }
                    }
                }

                replacement = ValueStringEx.GetValueString(value);
                if (useColor)
                {
                    replacement = replacement.ToValueString();
                }

                return replacement;
            }
            catch (Exception e)
            {
                Log.Error($"[{fieldName}] from [{typeof(T)}]: {e.Message}");
                return string.Empty;
            }
        }

        private static string LoadFloatValueString<T>(string fieldName, T passiveSettings, int level, int maxLevel, bool useColor, bool isShowNextLevel)
        {
            try
            {
                FieldInfo fieldInfo = typeof(T).GetField(fieldName);
                if (fieldInfo == null)
                {
                    Log.Error("패시브의 변수를 찾을 수 없습니다. {0}", fieldName);
                    return string.Empty;
                }

                string replacement;
                float value = (float)fieldInfo.GetValue(passiveSettings);
                if (maxLevel > 1)
                {
                    FieldInfo fieldInfoByLevel = typeof(T).GetField(fieldInfo.Name + "ByLevel");
                    float valueByLevel = (float)fieldInfoByLevel.GetValue(passiveSettings);
                    if (valueByLevel != 0)
                    {
                        if (isShowNextLevel)
                        {
                            float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            float nextValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                            return string.Format("{0} ▶ {1}", ValueStringEx.GetValueString(totalValue), ValueStringEx.GetValueString(nextValue, true));
                        }
                        else
                        {
                            float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            return ValueStringEx.GetValueString(totalValue, true);
                        }
                    }
                }

                replacement = ValueStringEx.GetValueString((int)value);
                if (useColor)
                {
                    replacement = replacement.ToValueString();
                }

                return replacement;
            }
            catch (Exception e)
            {
                Log.Error($"[{fieldName}] from [{typeof(T)}]: {e.Message}");
                return string.Empty;
            }
        }

        //

        private static string GetRewardCurrencyValues(PassiveRewardCurrency rewardCurrency, string fieldName)
        {
            FieldInfo fieldInfo = typeof(PassiveRewardCurrency).GetField(fieldName);
            if (fieldName == "Amount")
            {
                int value = (int)fieldInfo.GetValue(rewardCurrency);

                return ValueStringEx.GetValueString(value, GameColors.Value);
            }
            else if (fieldName == "Rate")
            {
                float value = (float)fieldInfo.GetValue(rewardCurrency);

                return ValueStringEx.GetPercentString(value, GameColors.Value);
            }

            return string.Empty;
        }
    }
}