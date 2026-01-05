using System.Text;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public static class EnumLogEx
    {
        private static string GetColorString(Color color, string content)
        {
#if UNITY_EDITOR

            if (content == "None")
            {
                return string.Format("<color={0}>{1}</color>", ColorEx.ColorToHex(GameColors.Gray), content);
            }
            else
            {
                return string.Format("<color={0}>{1}</color>", ColorEx.ColorToHex(color), content);
            }
#else
            return content;
#endif
        }

        public static string ToLogString(this StatNames key)
        {
            if (ScriptableDataManager.Instance.CheckLogWithLoadString())
            {
                string dataString = key.GetLocalizedString(LanguageNames.Korean);
                if (string.IsNullOrEmpty(dataString))
                {
                    return GetColorString(GameColors.Stat, key.ToString());
                }
                else
                {
                    dataString = dataString.ConvertStyleToColorString();
                    return GetColorString(GameColors.Stat, dataString);
                }
            }
            else
            {
                return GetColorString(GameColors.Stat, key.ToString());
            }
        }

        public static string ToLogString(this StatNames[] keys)
        {
            StringBuilder sb = new();

            for (int i = 0; i < keys.Length; i++)
            {
                StatNames key = keys[i];
                _ = sb.Append(key.ToLogString());
                _ = sb.Append(", ");
            }

            return sb.ToString();
        }

        public static string ToLogString(this StateEffects key)
        {
            return GetColorString(GameColors.Character, key.GetLocalizedString(LanguageNames.Korean));
        }

        public static string ToLogString(this CharacterNames key)
        {
            if (ScriptableDataManager.Instance.CheckLogWithLoadString())
            {
                string dataString = key.GetLocalizedString(LanguageNames.Korean);
                if (string.IsNullOrEmpty(dataString))
                {
                    return GetColorString(GameColors.Character, key.ToString());
                }
                else
                {
                    return GetColorString(GameColors.Character, dataString);
                }
            }
            else
            {
                return GetColorString(GameColors.Character, key.ToString());
            }
        }

        public static string ToLogString(this CharacterNames[] keys)
        {
            StringBuilder sb = new();

            for (int i = 0; i < keys.Length; i++)
            {
                CharacterNames key = keys[i];

                _ = sb.Append(key.ToLogString());

                if (i < keys.Length - 1)
                {
                    _ = sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        public static string ToLogString(this HitmarkNames key)
        {
            if (ScriptableDataManager.Instance.CheckLogWithLoadString())
            {
                string dataString = key.ToString();
                if (string.IsNullOrEmpty(dataString))
                {
                    return GetColorString(GameColors.Hitmark, key.ToString());
                }
                else
                {
                    return GetColorString(GameColors.Hitmark, dataString);
                }
            }
            else if (key != 0)
            {
                return GetColorString(GameColors.Hitmark, key.ToString());
            }
            else
            {
                return key.ToDisableString();
            }
        }

        public static string ToLogString(this HitmarkNames[] keys)
        {
            return GetColorString(GameColors.Hitmark, keys.JoinToString());
        }

        public static string ToLogString(this DamageTypes key)
        {
            if (ScriptableDataManager.Instance.CheckLogWithLoadString())
            {
                string dataString = key.ToString();
                if (string.IsNullOrEmpty(dataString))
                {
                    return GetColorString(GameColors.Hitmark, key.ToString());
                }
                else
                {
                    return GetColorString(GameColors.Hitmark, dataString);
                }
            }
            else if (key != 0)
            {
                return GetColorString(GameColors.Hitmark, key.ToString());
            }
            else
            {
                return key.ToDisableString();
            }
        }

        public static string ToLogString(this DamageTypes[] keys)
        {
            return GetColorString(GameColors.Hitmark, keys.JoinToString());
        }

        public static string ToLogString(this ItemNames key)
        {
            if (ScriptableDataManager.Instance.CheckLogWithLoadString())
            {
                string dataString = key.GetNameString(LanguageNames.Korean);
                if (string.IsNullOrEmpty(dataString))
                {
                    return GetColorString(GameColors.Item, key.ToString());
                }
                else
                {
                    return GetColorString(GameColors.Item, dataString);
                }
            }
            else
            {
                return GetColorString(GameColors.Item, key.ToString());
            }
        }

        public static string ToLogString(this GradeNames key)
        {
            string content = key.GetLocalizedString(LanguageNames.Korean);

            return content;
        }

        public static string ToLogString(this GradeNames[] keys)
        {
            StringBuilder sb = new();
            for (int i = 0; i < keys.Length; i++)
            {
                string content = keys[i].GetLocalizedString(LanguageNames.Korean);
                _ = sb.Append(content);
                if (i < keys.Length - 1)
                {
                    _ = sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        public static string ToLogString(this ItemTypes content)
        {
            return GetColorString(GameColors.Item, content.ToString());
        }

        // STAGE

        public static string ToLogString(this StageNames key)
        {
            if (ScriptableDataManager.Instance.CheckLogWithLoadString())
            {
                string content = key.GetLocalizedString(LanguageNames.Korean);
                if (!string.IsNullOrEmpty(content))
                {
                    return GetColorString(GameColors.LogStage, content.ToString());
                }
            }

            return key.ToString();
        }

        public static string ToLogString(this StageMoveTypes content)
        {
            return GetColorString(GameColors.LogStage, content.ToString());
        }

        public static string ToLogString(this MapTypes content)
        {
            return GetColorString(GameColors.LogStage, content.ToString());
        }

        public static string ToLogString(this UIPopupNames content)
        {
            return GetColorString(GameColors.UI, content.ToString());
        }
    }
}