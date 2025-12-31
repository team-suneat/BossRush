using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this BuffNames buffName)
        {
            return GetLocalizedString(buffName, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this BuffNames buffName, LanguageNames languageName)
        {
            BuffAsset asset = ScriptableDataManager.Instance.FindBuff(buffName);
            if (asset != null)
            {
                string key = $"Buff_Name_{buffName}";
                string content = JsonDataManager.FindStringClone(key, languageName);

                return content;
            }

            return buffName.ToString();
        }

        public static string GetValueDescString(this BuffNames buffName, string[] values)
        {
            BuffAsset asset = ScriptableDataManager.Instance.FindBuff(buffName);
            if (asset != null)
            {
                string key = $"Buff_Value_Desc_{buffName}";
                string format;
                format = JsonDataManager.FindStringClone(key);
                format = ReplaceCharacterName(format);
                format = ReplaceBuffName(format);
                format = ReplaceBuffValue(format, 1, true, false);
                format = ReplaceStatName(format);

                return Format(format, values, values.Length);
            }

            return buffName.ToString();
        }

        public static string GetValueDescString(this BuffNames buffName)
        {
            BuffAsset asset = ScriptableDataManager.Instance.FindBuff(buffName);
            if (asset != null)
            {
                string key = $"Buff_Value_Desc_{buffName}";
                string content;
                content = JsonDataManager.FindStringClone(key);
                content = ReplaceCharacterName(content);
                content = ReplaceBuffName(content);
                content = ReplaceBuffValue(content, 1, true, false);
                content = ReplaceStatName(content);

                return content;
            }

            return buffName.ToString();
        }

        public static string GetLocalizedString(this StateEffects key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this StateEffects key, LanguageNames languageName)
        {
            string stringKey = $"Buff_State_{key}";
            return JsonDataManager.FindStringClone(stringKey, languageName);
        }

        public static string GetString(this StateEffects[] keys, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] contents = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                StateEffects key = keys[i];
                contents[i] = GetLocalizedString(key, languageName);
            }

            return contents.JoinToString();
        }

        public static string GetDescString(this StateEffects stateEffect)
        {
            return GetDescString(stateEffect, GameSetting.Instance.Language.Name);
        }

        public static string GetDescString(this StateEffects stateEffect, LanguageNames languageName)
        {
            if (stateEffect == StateEffects.None)
            {
                return string.Empty;
            }

            string key = $"Buff_State_Desc_{stateEffect}";
            string content;

            content = JsonDataManager.FindStringClone(key, languageName);
            content = ReplaceCharacterName(content);
            content = ReplaceBuffValue(content, 1, true, false);
            content = ReplaceStatName(content);

            return content;
        }

        public static string GetDescString(this StateEffects stateEffect, string[] values)
        {
            if (stateEffect == StateEffects.None)
            {
                return string.Empty;
            }

            string key = $"Buff_State_Desc_{stateEffect}";
            string format = JsonDataManager.FindStringClone(key);
            format = ReplaceCharacterName(format);
            format = ReplaceBuffValue(format, 1, true, false);
            format = ReplaceStatName(format);

            string content = Format(format, values, values.Length);
            return content;
        }

        public static string GetStateEffectDescString(this BuffNames[] buffNames)
        {
            StringBuilder stringBuilder = new();
            int index = 0;

            for (int i = 0; i < buffNames.Length; i++)
            {
                if (buffNames[i] == BuffNames.None)
                {
                    continue;
                }

                string description = GetStateEffectDescString(buffNames[i]);
                if (!string.IsNullOrEmpty(description))
                {
                    if (index > 0)
                    {
                        index += 1;

                        stringBuilder.AppendLine();
                    }

                    stringBuilder.Append(description);
                }
            }

            return stringBuilder.ToString();
        }

        private static string GetStateEffectDescString(this BuffNames buffName)
        {
            BuffAsset buffAsset = ScriptableDataManager.Instance.FindBuff(buffName);
            if (buffAsset.IsValid())
            {
                return GetDescString(buffAsset.Data.StateEffect);
            }

            return string.Empty;
        }

        // Reflection

        public static string ReplaceBuffValue(string input, int level, bool useColor, bool isShowNextLevel)
        {
            string output;

            input = ReplaceStateEffect(input);
            output = ReplaceBuffValueWithThreeGroups(input, level, useColor, isShowNextLevel);
            output = ReplaceBuffValueWithTwoGroups(output, level, useColor, isShowNextLevel);

            return output;
        }

        public static string ReplaceBuffValue(string input, int level, float multiplier, bool useColor, bool isShowNextLevel)
        {
            string output;

            input = ReplaceStateEffect(input);
            output = ReplaceBuffValueWithThreeGroups(input, level, multiplier, useColor, isShowNextLevel);
            output = ReplaceBuffValueWithTwoGroups(output, level, multiplier, useColor, isShowNextLevel);

            return output;
        }

        private static string ReplaceBuffValueWithThreeGroups(string input, int level, bool useColor, bool isShowNextLevel)
        {
            /// [Buff.BuffName.StatValues[0]
            string pattern = @"\[Buff\.([a-zA-Z0-9_*]+)\.([a-zA-Z0-9_*]+)\[([0-9_*]+)\]\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string nameString = match.Groups[1].Value;
                    string fieldNameString = match.Groups[2].Value;
                    string fieldNumberString = match.Groups[3].Value;

                    BuffNames buffName = EnumEx.ConvertTo<BuffNames>(nameString);

                    if (buffName == BuffNames.None)
                    {
                        Log.Error("버프의 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    BuffAssetData buffAssetData = ScriptableDataManager.Instance.FindBuffClone(buffName);
                    if (!buffAssetData.IsValid())
                    {
                        Log.Error("버프의 에셋을 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    FieldInfo fieldInfo = typeof(BuffAssetData).GetField(fieldNameString);
                    if (fieldInfo == null)
                    {
                        Log.Error("버프의 변수를 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    if (!int.TryParse(fieldNumberString, out int fieldsIndex))
                    {
                        Log.Error("변수의 인덱스를 검사할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    object fieldObject = fieldInfo.GetValue(buffAssetData);
                    string replacement = string.Empty;

                    if (fieldNameString.Contains("StatValues"))
                    {
                        replacement = GetStatValues(fieldObject, fieldsIndex, buffAssetData, level, useColor, isShowNextLevel);
                    }
                    else if (fieldNameString.Contains("LinkedBuffStatDivisors"))
                    {
                        replacement = GetLinkedBuffStatDivisors(fieldObject, fieldsIndex, buffAssetData, useColor);
                    }
                    else if (fieldNameString.Contains("Ratio"))
                    {
                        float fieldValue = (float)fieldObject;
                        replacement = ValueStringEx.GetPercentString(fieldValue);

                        if (useColor)
                        {
                            replacement = string.Format("<style=Value>{0}</style>", replacement);
                        }

                        input = input.Replace(match.Value, replacement);
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

        private static string ReplaceBuffValueWithThreeGroups(string input, int level, float multiplier, bool useColor, bool isShowNextLevel)
        {
            /// [Buff.BuffName.StatValues[0]
            string pattern = @"\[Buff\.([a-zA-Z0-9_*]+)\.([a-zA-Z0-9_*]+)\[([0-9_*]+)\]\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string nameString = match.Groups[1].Value;
                    string fieldNameString = match.Groups[2].Value;
                    string fieldNumberString = match.Groups[3].Value;

                    BuffNames buffName = EnumEx.ConvertTo<BuffNames>(nameString);
                    if (buffName == BuffNames.None)
                    {
                        Log.Error("버프의 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    BuffAssetData buffAssetData = ScriptableDataManager.Instance.FindBuffClone(buffName);
                    if (!buffAssetData.IsValid())
                    {
                        Log.Error("버프의 에셋을 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    FieldInfo fieldInfo = typeof(BuffAssetData).GetField(fieldNameString);
                    if (fieldInfo == null)
                    {
                        Log.Error("버프의 변수를 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    if (!int.TryParse(fieldNumberString, out int fieldsIndex))
                    {
                        Log.Error("변수의 인덱스를 검사할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    object fieldObject = fieldInfo.GetValue(buffAssetData);
                    string replacement = string.Empty;

                    if (fieldNameString.Contains("StatValues"))
                    {
                        replacement = GetStatValues(fieldObject, fieldsIndex, buffAssetData, level, useColor, isShowNextLevel);
                    }
                    else if (fieldNameString.Contains("LinkedBuffStatDivisors"))
                    {
                        replacement = GetLinkedBuffStatDivisors(fieldObject, fieldsIndex, buffAssetData, useColor);
                    }
                    else if (fieldNameString.Contains("Ratio"))
                    {
                        float fieldValue = (float)fieldObject;
                        replacement = ValueStringEx.GetPercentString(fieldValue);

                        if (useColor)
                        {
                            replacement = string.Format("<style=Value>{0}</style>", replacement);
                        }

                        input = input.Replace(match.Value, replacement);
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

        private static string ReplaceBuffValueWithTwoGroups(string input, int level, bool useColor, bool isShowNextLevel)
        {
            /// [Buff.BuffName.Duration]
            string pattern = @"\[Buff\.([a-zA-Z0-9_]+)\.([a-zA-Z0-9_*]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string buffName = match.Groups[1].Value;
                    string fieldName = match.Groups[2].Value;

                    if (!Enum.TryParse(buffName, out BuffNames enumBuffName))
                    {
                        Log.Error("버프의 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    BuffAssetData assetData = ScriptableDataManager.Instance.FindBuffClone(enumBuffName);
                    if (!assetData.IsValid())
                    {
                        Log.Error("버프의 에셋을 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    FieldInfo fieldInfo = typeof(BuffAssetData).GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        Log.Error("버프의 변수를 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    object valueObject = fieldInfo.GetValue(assetData);
                    string replacement = string.Empty;

                    if (fieldName.Contains("Duration"))
                    {
                        replacement = GetFloatValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                    }
                    else if (fieldName.Contains("MaxStack"))
                    {
                        if (assetData.MaxStackByStat != StatNames.None)
                        {
                            if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null)
                            {
                                if (CharacterManager.Instance.Player.Stat.ContainsKey(assetData.MaxStackByStat))
                                {
                                    int maxStack = CharacterManager.Instance.Player.Stat.FindValueOrDefaultToInt(assetData.MaxStackByStat);
                                    if (useColor)
                                    {
                                        replacement = maxStack.ToSelectString();
                                    }
                                    else
                                    {
                                        replacement = maxStack.ToString();
                                    }
                                }
                                else
                                {
                                    replacement = GetIntValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                                }
                            }
                            else
                            {
                                replacement = GetIntValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                            }
                        }
                        else
                        {
                            replacement = GetIntValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                        }
                    }
                    else if (fieldName.Contains("Ratio"))
                    {
                        replacement = GetPercentValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                    }
                    else
                    {
                        if (fieldInfo.FieldType == typeof(float))
                        {
                            replacement = Mathf.Abs((float)valueObject).ToString();
                        }
                        else
                        {
                            replacement = valueObject.ToString();
                        }

                        if (useColor)
                        {
                            replacement = "<style=Value>" + replacement + "</style>";
                        }
                    }

                    input = input.Replace(match.Value, replacement);
                }
            }
            return input;
        }

        private static string ReplaceBuffValueWithTwoGroups(string input, int level, float multiplier, bool useColor, bool isShowNextLevel)
        {
            /// [Buff.BuffName.Duration]
            string pattern = @"\[Buff\.([a-zA-Z0-9_]+)\.([a-zA-Z0-9_*]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string buffName = match.Groups[1].Value;
                    string fieldName = match.Groups[2].Value;

                    if (!Enum.TryParse(buffName, out BuffNames enumBuffName))
                    {
                        Log.Error("버프의 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    BuffAssetData assetData = ScriptableDataManager.Instance.FindBuffClone(enumBuffName);
                    if (!assetData.IsValid())
                    {
                        Log.Error("버프의 에셋을 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    FieldInfo fieldInfo = typeof(BuffAssetData).GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        Log.Error("버프의 변수를 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    object valueObject = fieldInfo.GetValue(assetData);
                    string replacement = string.Empty;

                    if (fieldName.Contains("Duration"))
                    {
                        replacement = GetFloatValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                    }
                    else if (fieldName.Contains("MaxStack"))
                    {
                        if (assetData.MaxStackByStat != StatNames.None)
                        {
                            if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null)
                            {
                                if (CharacterManager.Instance.Player.Stat.ContainsKey(assetData.MaxStackByStat))
                                {
                                    int maxStack = CharacterManager.Instance.Player.Stat.FindValueOrDefaultToInt(assetData.MaxStackByStat);
                                    if (useColor)
                                    {
                                        replacement = maxStack.ToSelectString();
                                    }
                                    else
                                    {
                                        replacement = maxStack.ToString();
                                    }
                                }
                                else
                                {
                                    replacement = GetIntValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                                }
                            }
                            else
                            {
                                replacement = GetIntValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                            }
                        }
                        else
                        {
                            replacement = GetIntValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                        }
                    }
                    else if (fieldName.Contains("Ratio"))
                    {
                        replacement = GetPercentValue(fieldInfo, assetData, level, useColor, isShowNextLevel);
                    }
                    else
                    {
                        if (fieldInfo.FieldType == typeof(float))
                        {
                            replacement = Mathf.Abs((float)valueObject).ToString();
                        }
                        else
                        {
                            replacement = valueObject.ToString();
                        }

                        if (useColor)
                        {
                            replacement = "<style=Value>" + replacement + "</style>";
                        }
                    }

                    input = input.Replace(match.Value, replacement);
                }
            }
            return input;
        }

        //

        private static string GetIntValue(FieldInfo fieldInfo, BuffAssetData assetData, int level, bool useColor, bool isShowNextLevel)
        {
            int value = (int)fieldInfo.GetValue(assetData);

            string replacement;
            if (assetData.MaxLevel > 1)
            {
                FieldInfo fieldInfo2 = typeof(BuffAssetData).GetField(fieldInfo.Name + "ByLevel");
                if (fieldInfo2 != null)
                {
                    int valueByLevel = (int)fieldInfo2.GetValue(assetData);
                    if (valueByLevel == 0)
                    {
                        if (useColor)
                        {
                            replacement = value.ToValueString();
                        }
                        else
                        {
                            replacement = value.ToString();
                        }
                    }
                    else if (isShowNextLevel && level + 1 <= assetData.MaxLevel)
                    {
                        int totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        int nextValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                        return string.Format("{0} ▶ {1}", ValueStringEx.GetValueString(totalValue), ValueStringEx.GetValueString(nextValue, useColor));
                    }
                    else
                    {
                        int totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        replacement = ValueStringEx.GetValueString(totalValue, useColor);
                    }
                }
                else
                {
                    if (useColor)
                    {
                        replacement = value.ToValueString();
                    }
                    else
                    {
                        replacement = value.ToString();
                    }
                }
            }
            else
            {
                if (useColor)
                {
                    replacement = value.ToValueString();
                }
                else
                {
                    replacement = value.ToString();
                }
            }

            return replacement;
        }

        private static string GetFloatValue(FieldInfo fieldInfo, BuffAssetData assetData, int level, bool useColor, bool isShowNextLevel)
        {
            float value = (float)fieldInfo.GetValue(assetData);

            string replacement;
            if (assetData.MaxLevel > 1)
            {
                FieldInfo fieldInfo2 = typeof(BuffAssetData).GetField(fieldInfo.Name + "ByLevel");
                if (fieldInfo2 != null)
                {
                    float valueByLevel = (float)fieldInfo2.GetValue(assetData);
                    if (valueByLevel.IsZero())
                    {
                        if (useColor)
                        {
                            replacement = value.ToValueString();
                        }
                        else
                        {
                            replacement = value.ToString();
                        }
                    }
                    else if (isShowNextLevel && level + 1 <= assetData.MaxLevel)
                    {
                        float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        float nextValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                        return string.Format("{0} ▶ {1}", ValueStringEx.GetValueString(totalValue), ValueStringEx.GetValueString(nextValue, useColor));
                    }
                    else
                    {
                        float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        replacement = ValueStringEx.GetValueString(totalValue, useColor);
                    }
                }
                else
                {
                    if (useColor)
                    {
                        replacement = value.ToValueString();
                    }
                    else
                    {
                        replacement = value.ToString();
                    }
                }
            }
            else
            {
                if (useColor)
                {
                    replacement = value.ToValueString();
                }
                else
                {
                    replacement = value.ToString();
                }
            }

            return replacement;
        }

        private static string GetPercentValue(FieldInfo fieldInfo, BuffAssetData assetData, int level, bool useColor, bool isShowNextLevel)
        {
            float value = (float)fieldInfo.GetValue(assetData);

            string replacement;
            if (assetData.MaxLevel > 1)
            {
                FieldInfo fieldInfo2 = typeof(BuffAssetData).GetField(fieldInfo.Name + "ByLevel");
                if (fieldInfo2 != null)
                {
                    float valueByLevel = (float)fieldInfo2.GetValue(assetData);
                    if (valueByLevel.IsZero())
                    {
                        if (useColor)
                        {
                            replacement = ValueStringEx.GetPercentString(value, GameColors.Value);
                        }
                        else
                        {
                            replacement = ValueStringEx.GetPercentString(value);
                        }
                    }
                    else if (isShowNextLevel && level + 1 <= assetData.MaxLevel)
                    {
                        float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        float nextValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                        return string.Format("{0} ▶ {1}", ValueStringEx.GetPercentString(totalValue), ValueStringEx.GetPercentString(nextValue, useColor));
                    }
                    else
                    {
                        float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        replacement = ValueStringEx.GetPercentString(totalValue, useColor);
                    }
                }
                else
                {
                    if (useColor)
                    {
                        replacement = ValueStringEx.GetPercentString(value, GameColors.Value);
                    }
                    else
                    {
                        replacement = ValueStringEx.GetPercentString(value);
                    }
                }
            }
            else
            {
                if (useColor)
                {
                    replacement = ValueStringEx.GetPercentString(value, GameColors.Value);
                }
                else
                {
                    replacement = ValueStringEx.GetPercentString(value);
                }
            }

            return replacement;
        }

        private static string GetStatValues(object fieldObject, int fieldsIndex, BuffAssetData buffAssetData, int level, bool useColor, bool isShowNextLevel)
        {
            float[] values = (float[])fieldObject;
            if (values.Length <= fieldsIndex)
            {
                LogFailedToFindStat(buffAssetData, values, fieldsIndex);
                return fieldObject.ToString();
            }

            if (buffAssetData.Stats.Length <= fieldsIndex)
            {
                LogFailedToFindStat(buffAssetData, fieldsIndex);
                return fieldObject.ToString();
            }

            float statValue = values[fieldsIndex];
            StatNames statName = buffAssetData.Stats[fieldsIndex];

            if (buffAssetData.MaxLevel > 1)
            {
                if (buffAssetData.StatValuesByLevel.Length > fieldsIndex)
                {
                    float statValueByLevel = buffAssetData.StatValuesByLevel[fieldsIndex];
                    if (!statValueByLevel.IsZero())
                    {
                        if (isShowNextLevel && level + 1 <= buffAssetData.MaxLevel)
                        {
                            float totalValue = StatEx.GetValueByLevel(statValue, statValueByLevel, level);
                            float nextValue = StatEx.GetValueByLevel(statValue, statValueByLevel, level + 1);

                            return string.Format("{0} ▶ {1}",
                                statName.GetStatValueString(totalValue),
                                statName.GetStatValueString(nextValue, useColor));
                        }
                        else
                        {
                            float resultStatValue = StatEx.GetValueByLevel(statValue, statValueByLevel, level);
                            string content = statName.GetStatValueString(Mathf.Abs(resultStatValue));
                            if (useColor)
                            {
                                if (resultStatValue < 0 && buffAssetData.UseStatRedColor)
                                {
                                    return content.ToErrorString();
                                }
                                else
                                {
                                    return content.ToSelectString();
                                }
                            }
                        }
                    }
                }
            }

            string valueContent = statName.GetStatValueString(Mathf.Abs(statValue));
            if (useColor)
            {
                if (statValue < 0 && buffAssetData.UseStatRedColor)
                {
                    return valueContent.ToErrorString();
                }
                else
                {
                    return valueContent.ToValueString();
                }
            }

            return valueContent;
        }

        private static string GetLinkedBuffStatDivisors(object fieldObject, int fieldsIndex, BuffAssetData buffAssetData, bool useColor)
        {
            float[] values = (float[])fieldObject;
            if (values.Length > fieldsIndex)
            {
                float divisor = values[fieldsIndex];
                LinkedBuffStatTypes type = buffAssetData.LinkedBuffStatTypes[fieldsIndex];

                if (type == LinkedBuffStatTypes.None)
                {
                    Log.Error("버프의 연결된 능력치 값 종류가 설정되지 않았습니다. {0}", buffAssetData.Name.ToLogString());
                }
                if (type is LinkedBuffStatTypes.MissingHealthOfAttacker)
                {
                    if (useColor)
                    {
                        return ValueStringEx.GetPercentString(divisor).AddStyleString("Value");
                    }
                    else
                    {
                        return ValueStringEx.GetPercentString(divisor);
                    }
                }
                else
                {
                    if (useColor)
                    {
                        return ValueStringEx.GetValueString(divisor).AddStyleString("Value");
                    }
                    else
                    {
                        return ValueStringEx.GetValueString(divisor);
                    }
                }
            }
            else
            {
                Log.Error("버프의 변수의 수가 원하는 인덱스보다 적습니다. Length:{0}, Index:{1}", values.Length, fieldsIndex);
                return fieldObject.ToString();
            }
        }

        //

        private static void LogFailedToFindStat(BuffAssetData buffAssetData, float[] statValues, int fieldsIndex)
        {
            if (Log.LevelError)
            {
                Log.Error("버프({0})의 능력치 값({1})을 찾을 수 없습니다. Length:{2}, Index:{3}",
                    buffAssetData.Name.ToLogString(), statValues.JoinToString(),
                    statValues.Length, fieldsIndex);
            }
        }

        private static void LogFailedToFindStat(BuffAssetData buffAssetData, int fieldsIndex)
        {
            if (Log.LevelError)
            {
                Log.Error("버프({0})의 능력치({1})를 찾을 수 없습니다. Length:{2}, Index:{3}",
                     buffAssetData.Name.ToLogString(), buffAssetData.Stats.ToLogString(),
                     buffAssetData.Stats.Length, fieldsIndex);
            }
        }
    }
}