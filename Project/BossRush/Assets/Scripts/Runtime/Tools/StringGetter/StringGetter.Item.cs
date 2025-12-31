using System.Text;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this ItemNames key)
        {
            return GetNameString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetNameString(this ItemNames key, LanguageNames languageName)
        {
            if (key == ItemNames.None)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Item_Name_");
            stringBuilder.Append(key.ToString());

            string result = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);

            if (string.IsNullOrEmpty(result))
            {
                Log.Warning($"아이템 이름({key})을 찾을 수 없습니다.");
                return key.ToString();
            }

            return result;
        }

        //

        public static string GetDescString(this ItemNames key)
        {
            return GetDescString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetDescString(this ItemNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Item_Desc_");
            stringBuilder.Append(key.ToString());

            string content = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);
            if (!string.IsNullOrEmpty(content))
            {
                content = ReplaceStatName(content);
            }
            return content;
        }

        public static string GetDescString(this ItemNames key, string[] values)
        {
            return GetDescString(key, values, GameSetting.Instance.Language.Name);
        }

        public static string GetDescString(this ItemNames key, string[] values, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Item_Desc_");
            stringBuilder.Append(key.ToString());

            StringData stringData = JsonDataManager.FindStringData(stringBuilder.ToString());
            if (stringData.IsValid())
            {
                string content = stringData.GetString(languageName);
                if (values.IsValid())
                {
                    return Format(content, values, stringData.Arguments);
                }
                else
                {
                    return content;
                }
            }

            Log.Warning(LogTags.String, "스트링 데이터를 찾을 수 없습니다. {0}", stringBuilder.ToString());

            return string.Empty;
        }

        //

        public static string GetSubDescString(this ItemNames key)
        {
            return GetSubDescString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetSubDescString(this ItemNames key, int value)
        {
            return GetSubDescString(key, value, GameSetting.Instance.Language.Name);
        }

        public static string GetSubDescString(this ItemNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SubDesc_");
            stringBuilder.Append(key.ToString());

            string content = JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);
            if (!string.IsNullOrEmpty(content))
            {
                return ReplaceItemSubDesc(content);
            }

            return content;
        }

        public static string GetSubDescString(this ItemNames key, int value, LanguageNames languageName)
        {
            string format = key.GetSubDescString(languageName);
            return string.Format(format, value);
        }

        // Notice

        public static string GetObtainNoticeString(this string value)
        {
            string format = JsonDataManager.FindStringClone("Notice_Obtain_Item1");
            string content = string.Format(format, value);

            return content;
        }

        public static string GetObtainNoticeString(this string value, GradeNames gradeName)
        {
            string format = JsonDataManager.FindStringClone("Notice_Obtain_Item2");
            string content = string.Format(format, gradeName.ToString(), value);

            return content;
        }

        public static string GetObtainNoticeString(this ItemNames key)
        {
            string format = JsonDataManager.FindStringClone("Notice_Obtain_Item1");
            string value = string.Format(format, key.GetLocalizedString());

            return value;
        }

        public static string GetObtainNoticeString(this ItemNames key, GradeNames gradeName)
        {
            string format = JsonDataManager.FindStringClone("Notice_Obtain_Item2");
            string value = string.Format(format, gradeName.ToString(), key.GetLocalizedString());

            return value;
        }

        public static string GetUseNoticeString(this string value)
        {
            string format = JsonDataManager.FindStringClone("Notice_Use_Item1");
            string content = string.Format(format, value.AddStyleString("Value"));

            return content;
        }

        // 재화 (Currency)
        public static string GetStringKey(this CurrencyNames key)
        {
            return $"Currency_Name_{key}";
        }

        public static string GetLocalizedString(this CurrencyNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this CurrencyNames key, LanguageNames languageName)
        {
            return JsonDataManager.FindStringClone($"Currency_Name_{key}", languageName);
        }

        public static string GetFormatString(this CurrencyNames key)
        {
            return JsonDataManager.FindStringClone($"Currency_Format_{key}");
        }

        public static string GetLocalizedString(this CurrencyNames key, int value)
        {
            return string.Format(GetFormatString(key), value);
        }

        public static string GetDescString(this CurrencyNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Currency_Desc_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        // 장비 (Equipment)

        public static string GetGambleString(this EquipmentSlotTypes key)
        {
            return JsonDataManager.FindStringClone(key.ToString());
        }

        public static string GetLocalizedString(this GradeNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this GradeNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Grade_Name_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString(), languageName);
        }

        public static string ReplaceItemSubDesc(string content)
        {
            content = ReplaceStatName(content);
            content = ReplaceAreaName(content);
            content = ReplaceStageName(content);

            content = ReplaceMonsterName(content);
            content = ReplaceItemName(content);

            return content;
        }
    }
}