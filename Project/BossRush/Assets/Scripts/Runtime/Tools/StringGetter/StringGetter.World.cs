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

        public static string GetLocalizedString(this AreaNames key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this AreaNames key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Area_Name_");
            stringBuilder.Append(key.ToString());

            string stringKey = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(stringKey, languageName);
        }

        public static string GetNotStyleString(this AreaNames areaName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Area_Name_");
            stringBuilder.Append(areaName.ToString());

            string key = stringBuilder.ToString();
            string value = JsonDataManager.FindStringClone(key);

            return ConvertStyleToColorString(value);
        }

        //

        public static string GetDescString(this StageNames stageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Stage_Description_");
            stringBuilder.Append(stageName.ToString());

            string key = stringBuilder.ToString();
            string content = JsonDataManager.FindStringClone(key);
            if (!string.IsNullOrEmpty(content))
            {
                content = ReplaceAreaName(content);
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
                content = ReplaceAreaName(content);
                content = ReplaceStageName(content);

                content = ReplaceMonsterName(content);
            }

            return ConvertStyleToColorString(content);
        }

        public static string GetDescString(this AreaNames areaName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Area_Description_");
            stringBuilder.Append(areaName.ToString());

            string key = stringBuilder.ToString();
            string content = JsonDataManager.FindStringClone(key);
            if (!string.IsNullOrEmpty(content))
            {
                content = ReplaceAreaName(content);
                content = ReplaceStageName(content);

                content = ReplaceMonsterName(content);
            }

            return content;
        }

        public static string GetNotStyleDescString(this AreaNames areaName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Area_Description_");
            stringBuilder.Append(areaName.ToString());

            string key = stringBuilder.ToString();
            string content = JsonDataManager.FindStringClone(key);
            if (!string.IsNullOrEmpty(content))
            {
                content = ReplaceAreaName(content);
                content = ReplaceStageName(content);

                content = ReplaceMonsterName(content);
            }

            return ConvertStyleToColorString(content);
        }

        //

        //

        public static string GetLocalizedString(this DifficultyEnums key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this DifficultyEnums key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Name_");
            stringBuilder.Append(key.ToString());

            string stringKey = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(stringKey, languageName);
        }

        public static string GetDescString(this DifficultyEnums difficulty)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Desc_");
            stringBuilder.Append(difficulty.ToString());

            string key = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(key);
        }

        public static string GetEffectString(this DifficultyEnums difficulty, int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Effect_");
            stringBuilder.Append(difficulty.ToString());
            stringBuilder.Append(index.ToString());

            string key = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(key);
        }

        public static string GetDescString(this DifficultyEnums difficulty, int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Desc_");
            stringBuilder.Append(difficulty.ToString());
            stringBuilder.Append("_");
            stringBuilder.Append(index.ToString());

            string key = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(key);
        }

        public static string GetRewardString(this DifficultyEnums difficulty, int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Reward_");
            stringBuilder.Append(difficulty.ToString());
            stringBuilder.Append(index.ToString());

            string key = stringBuilder.ToString();
            string content;
            content = JsonDataManager.FindStringClone(key);
            content = ReplaceCurrencyName(content);
            content = ReplaceAreaName(content);
            return content;
        }

        //

        public static string GetLocalizedString(this DifficultyStepModes key)
        {
            return GetLocalizedString(key, GameSetting.Instance.Language.Name);
        }

        public static string GetLocalizedString(this DifficultyStepModes key, LanguageNames languageName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Step_Name_");
            stringBuilder.Append(key.ToString());

            string stringKey = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(stringKey, languageName);
        }

        public static string GetEffectString(this DifficultyStepModes difficultyStep, int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Effect_");
            stringBuilder.Append(difficultyStep.ToString());
            stringBuilder.Append(index.ToString());

            string key = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(key);
        }

        public static string GetDeacString(this DifficultyStepModes stepMode)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Difficulty_Step_Desc_");
            stringBuilder.Append(stepMode.ToString());

            string key = stringBuilder.ToString();
            return JsonDataManager.FindStringClone(key);
        }
    }
}