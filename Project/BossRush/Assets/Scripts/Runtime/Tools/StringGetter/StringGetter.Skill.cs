using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this SkillNames skillName)
        {
            return GetLocalizedString(skillName, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this SkillNames skillName, LanguageNames languageName)
        {
            string key = $"Skill_Name_{skillName}";
            string content = JsonDataManager.FindStringClone(key, languageName);

            return content;
        }
    }
}