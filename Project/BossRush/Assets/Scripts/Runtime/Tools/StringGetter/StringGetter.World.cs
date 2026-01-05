using System.Text;
using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetStringKey(this StageNames key)
        {
            return $"Stage_Name_{key}";
        }

        public static string GetLocalizedString(this StageNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this StageNames key, LanguageNames languageName)
        {
            string stringKey = GetStringKey(key);
            return JsonDataManager.FindStringClone(stringKey, languageName);
        }

        public static string GetNotStyleString(this StageNames stageName)
        {
            string key = GetStringKey(stageName);
            string value = JsonDataManager.FindStringClone(key);

            return ConvertStyleToColorString(value);
        }

        public static string GetDescString(this StageNames stageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Stage_Description_");
            stringBuilder.Append(stageName.ToString());

            string key = stringBuilder.ToString();
            string content = JsonDataManager.FindStringClone(key);
            if (!string.IsNullOrEmpty(content))
            {
                
                content = ReplaceStageName(content);

                content = ReplaceMonsterName(content);
            }

            return content;
        }

        public static string GetNotStyleDescString(this StageNames stageName, bool isNotStyle = false)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Stage_Description_");
            stringBuilder.Append(stageName.ToString());

            string key = stringBuilder.ToString();
            string content = JsonDataManager.FindStringClone(key);
            if (!string.IsNullOrEmpty(content))
            {
                
                content = ReplaceStageName(content);

                content = ReplaceMonsterName(content);
            }

            return ConvertStyleToColorString(content);
        }
    }
}