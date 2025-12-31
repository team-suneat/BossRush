using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetStringKey(this CharacterNames key)
        {
            return $"Character_Name_{key}";
        }

        public static string GetLocalizedString(this CharacterNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this CharacterNames key, LanguageNames languageName)
        {
            string stringKey = $"Character_Name_{key}";
            return JsonDataManager.FindStringClone(stringKey, languageName);
        }

        public static string GetDescString(this CharacterNames key)
        {
            string stringKey = $"Character_Desc_{key}";
            return JsonDataManager.FindStringClone(stringKey);
        }

        //
    }
}