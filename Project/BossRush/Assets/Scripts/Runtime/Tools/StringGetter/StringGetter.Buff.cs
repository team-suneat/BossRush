using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetStringKey(this StateEffects key)
        {
            return $"StateEffect_Name_{key.ToString()}";
        }

        public static string GetLocalizedString(this StateEffects key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this StateEffects key, LanguageNames languageName)
        {
            string result = JsonDataManager.FindStringClone(key.GetStringKey(), languageName);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return key.ToString();
        }
    }
}