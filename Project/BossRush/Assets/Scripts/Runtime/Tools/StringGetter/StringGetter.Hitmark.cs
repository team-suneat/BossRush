using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        // Reflection

        /// <summary> 히트마크 에셋 데이터에 있는 변수 값을 가져옵니다. </summary>
        public static string ReplaceHitmarkValue(string input, int level, bool useColor, bool isShowNextLevel)
        {
            string output;

            output = ReplaceHitmarkValueWithFourGroups(input, level, isShowNextLevel);
            output = ReplaceHitmarkValueWithThreeGroups(output, level, useColor, isShowNextLevel);
            output = ReplaceHitmarkValueWithTwoGroups(output, level, useColor, isShowNextLevel);

            return output;
        }

        private static string ReplaceHitmarkValueWithFourGroups(string input, int level, bool isShowNextLevel)
        {
            /// [Hitmark.Hitmark.FieldName[FieldIndex].FieldName2]]
            /// 예시: [Hitmark.WeaponAttack.Damages[0].DamageRatio]]
            string pattern = @"\[Hitmark\.([a-zA-Z0-9_*]+)\.([a-zA-Z0-9_*]+)\[([0-9_*]+)\]\.([a-zA-Z0-9_*]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string nameString = match.Groups[1].Value;
                    string fieldNameString = match.Groups[2].Value;
                    string fieldNumberString = match.Groups[3].Value;
                    string fieldName2String = match.Groups[4].Value;

                    HitmarkNames hitmarkName = EnumEx.ConvertTo<HitmarkNames>(nameString);
                    if (hitmarkName == HitmarkNames.None)
                    {
                        Log.Error("히트마크의 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    HitmarkAssetData hitmarkAssetData = ScriptableDataManager.Instance.FindHitmarkClone(hitmarkName);
                    if (!hitmarkAssetData.IsValid())
                    {
                        Log.Error("히트마크의 에셋을 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    FieldInfo fieldInfo = typeof(HitmarkAssetData).GetField(fieldNameString);
                    if (fieldInfo == null)
                    {
                        Log.Error("히트마크의 변수를 찾을 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    if (!int.TryParse(fieldNumberString, out int fieldsIndex))
                    {
                        Log.Error("변수의 인덱스를 검사할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    object fieldObject = fieldInfo.GetValue(hitmarkAssetData);
                    string replacement = string.Empty;
                    if (fieldNameString.Contains("Damages"))
                    {
                        HitmarkAssetData[] datas = (HitmarkAssetData[])fieldObject;
                        if (datas != null && datas.Length > fieldsIndex)
                        {
                            replacement = GetDamageValues(datas[fieldsIndex], fieldName2String, level, isShowNextLevel);
                        }
                        else
                        {
                            Log.Error("접근하려는 히트마크({0})의 {1}번째 피해 에셋 데이터를 찾을 수 없습니다.", hitmarkName.ToLogString(), fieldsIndex);
                        }
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

        private static string ReplaceHitmarkValueWithThreeGroups(string input, int level, bool useColor, bool isShowNextLevel)
        {
            string pattern = @"\[Hitmark\.([a-zA-Z0-9_]+)\.([a-zA-Z0-9_]+)\.([a-zA-Z0-9_*]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string hitmarkName = match.Groups[1].Value;
                    string damageNumber = match.Groups[2].Value.Replace("Damage", "");
                    string fieldName = match.Groups[3].Value;

                    float multiplier = 1f;
                    int firstIndex = fieldName.IndexOf('*');
                    if (firstIndex > 0)
                    {
                        int LastIndex = fieldName.LastIndexOf('*');
                        multiplier += LastIndex - firstIndex;
                        fieldName = fieldName.Replace("*", "");
                    }

                    if (!int.TryParse(damageNumber, out int damageIndex))
                    {
                        continue;
                    }
                    damageIndex -= 1;

                    if (!Enum.TryParse(hitmarkName, out HitmarkNames enumHitmarkName))
                    {
                        continue;
                    }

                    HitmarkAssetData assetData = ScriptableDataManager.Instance.FindHitmarkClone(enumHitmarkName);
                    if (!assetData.IsValid())
                    {
                        continue;
                    }

                FieldInfo fieldInfo = typeof(HitmarkAssetData).GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        continue;
                    }

                HitmarkAssetData damage = assetData;
                object fieldObject = fieldInfo.GetValue(damage);
                    string replacement;

                    if (fieldInfo.FieldType == typeof(float))
                    {
                        float fieldValue = (float)fieldObject;

                        if (fieldName.Contains("Ratio") || fieldName.Contains("Rate") || fieldName.Contains("Magnification") || fieldName.Contains("Multiplier"))
                        {
                            replacement = ValueStringEx.GetPercentString(fieldValue * multiplier);
                            if (useColor)
                            {
                                replacement = string.Format("<style=Value>{0}</style>", replacement);
                            }
                        }
                        else
                        {
                            replacement = ValueStringEx.GetValueString(fieldValue * multiplier);
                            replacement = string.Format("<style=Value>{0}</style>", replacement);
                        }
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

        private static string ReplaceHitmarkValueWithTwoGroups(string input, int level, bool useColor, bool isShowNextLevel)
        {
            /// Hitmark.HitmarkName.DataValueName
            string pattern = @"\[Hitmark\.([a-zA-Z0-9_]+)\.([a-zA-Z0-9_*]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string hitmarkName = match.Groups[1].Value;
                    string fieldName = match.Groups[2].Value;

                    float multiplier = 1f;
                    int firstIndex = fieldName.IndexOf('*');
                    if (firstIndex > 0)
                    {
                        int LastIndex = fieldName.LastIndexOf('*');
                        multiplier += LastIndex - firstIndex;
                        fieldName = fieldName.Replace("*", "");
                    }

                    if (!Enum.TryParse(hitmarkName, out HitmarkNames enumHitmarkName))
                    {
                        continue;
                    }

                    HitmarkAssetData assetData = ScriptableDataManager.Instance.FindHitmarkClone(enumHitmarkName);
                    if (!assetData.IsValid())
                    {
                        continue;
                    }

                    FieldInfo fieldInfo = typeof(HitmarkAssetData).GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        continue;
                    }

                    object fieldObject = fieldInfo.GetValue(assetData);
                    string replacement;

                    if (fieldInfo.FieldType == typeof(float))
                    {
                        float fieldValue = (float)fieldObject;
                        if (fieldName.Contains("Ratio") || fieldName.Contains("Rate") || fieldName.Contains("Magnification") || fieldName.Contains("Multiplier"))
                        {
                            replacement = ValueStringEx.GetPercentString(fieldValue * multiplier);

                            if (useColor)
                            {
                                replacement = string.Format("<style=Value>{0}</style>", replacement);
                            }
                        }
                        else if (fieldName.Contains("UseResourceValue") || fieldName.Contains("RestoreResourceValue"))
                        {
                            switch (assetData.ResourceConsumeType)
                            {
                                case VitalConsumeTypes.MaxHealthPercent:
                                case VitalConsumeTypes.MaxShieldPercent:

                                case VitalConsumeTypes.CurrentHealthPercent:
                                case VitalConsumeTypes.CurrentShieldPercent:
                                    {
                                        replacement = ValueStringEx.GetPercentString(fieldValue * multiplier);
                                    }
                                    break;

                                case VitalConsumeTypes.FixedHealth:
                                case VitalConsumeTypes.FixedShield:
                                default:
                                    {
                                        replacement = ValueStringEx.GetValueString(fieldValue * multiplier);
                                    }
                                    break;
                            }

                            if (useColor)
                            {
                                replacement = string.Format("<style=Value>{0}</style>", replacement);
                            }
                        }
                        else
                        {
                            replacement = ValueStringEx.GetValueString(fieldValue * multiplier);

                            if (useColor)
                            {
                                replacement = string.Format("<style=Value>{0}</style>", replacement);
                            }
                        }
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

        //

        private static string GetDamageValues(HitmarkAssetData damageAssetData, string fieldName, int level, bool isShowNextLevel)
        {
            FieldInfo fieldInfo = typeof(HitmarkAssetData).GetField(fieldName);
            FieldInfo fieldInfoByLevel = typeof(HitmarkAssetData).GetField(fieldName + "ByLevel");

            if (fieldInfo == null)
            {
                Log.Error("HitmarkAssetData에서 필드({0})를 찾을 수 없습니다.", fieldName);
                return string.Empty;
            }

            float value = (float)fieldInfo.GetValue(damageAssetData);
            float valueByLevel = 0f;
            if (fieldInfoByLevel != null)
            {
                valueByLevel = (float)fieldInfoByLevel.GetValue(damageAssetData);
            }

            bool useColor = !valueByLevel.IsZero();

            if (fieldName.Contains("Ratio") || fieldName.Contains("Rate") || fieldName.Contains("Magnification") || fieldName.Contains("Multiplier"))
            {
                if (!valueByLevel.IsZero())
                {
                    if (isShowNextLevel)
                    {
                        float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        float nextLevelValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                        return string.Format("{0} ▶ {1}", ValueStringEx.GetPercentString(totalValue), ValueStringEx.GetPercentString(nextLevelValue, true));
                    }
                    else
                    {
                        float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                        return ValueStringEx.GetPercentString(totalValue, useColor);
                    }
                }

                if (useColor)
                {
                    return ValueStringEx.GetPercentString(value, true);
                }
                else
                {
                    return ValueStringEx.GetPercentString(value, GameColors.Value);
                }
            }
            else
            {
                if (fieldInfoByLevel != null)
                {
                    if (!valueByLevel.IsZero())
                    {
                        if (isShowNextLevel)
                        {
                            float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            float nextLevelValue = StatEx.GetValueByLevel(value, valueByLevel, level + 1);

                            return string.Format("{0} ▶ {1}", ValueStringEx.GetValueString(totalValue), ValueStringEx.GetValueString(nextLevelValue, true));
                        }
                        else
                        {
                            float totalValue = StatEx.GetValueByLevel(value, valueByLevel, level);
                            return ValueStringEx.GetValueString(totalValue, useColor);
                        }
                    }
                }

                if (useColor)
                {
                    return ValueStringEx.GetValueString(value, true);
                }
                else
                {
                    return ValueStringEx.GetValueString(value, GameColors.Value);
                }
            }
        }
    }
}