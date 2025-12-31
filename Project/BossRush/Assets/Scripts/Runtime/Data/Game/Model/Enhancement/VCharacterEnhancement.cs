using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// 캐릭터의 강화 능력치 레벨 저장 데이터
    /// </summary>
    [System.Serializable]
    public class VCharacterEnhancement
    {
        /// <summary>
        /// 강화 능력치별 레벨 (string -> Level)
        /// </summary>
        public Dictionary<string, int> EnhancementLevels = new Dictionary<string, int>();

        public void OnLoadGameData()
        {
            if (EnhancementLevels == null)
            {
                EnhancementLevels = new Dictionary<string, int>();
            }

            Log.Info(LogTags.GameData_Character, "강화 능력치 레벨 데이터를 불러옵니다. 총 {0}개", EnhancementLevels.Count);
        }

        /// <summary>
        /// 특정 강화 능력치의 레벨을 가져옵니다.
        /// </summary>
        public int GetLevel(StatNames statName)
        {
            if (EnhancementLevels == null)
            {
                return 0;
            }

            string key = statName.ToString();
            return EnhancementLevels.TryGetValue(key, out int level) ? level : 0;
        }

        /// <summary>
        /// 특정 강화 능력치의 레벨을 설정합니다.
        /// </summary>
        public void SetLevel(StatNames statName, int level)
        {
            if (EnhancementLevels == null)
            {
                EnhancementLevels = new Dictionary<string, int>();
            }

            string key = statName.ToString();
            EnhancementLevels[key] = level;
            Log.Info(LogTags.GameData_Character, "강화 능력치 {0}의 레벨을 {1}로 설정합니다.", statName, level);
        }

        /// <summary>
        /// 특정 강화 능력치의 레벨을 증가시킵니다.
        /// </summary>
        public int AddLevel(StatNames statName, int addLevel = 1)
        {
            int currentLevel = GetLevel(statName);
            int newLevel = currentLevel + addLevel;
            SetLevel(statName, newLevel);
            GlobalEvent<StatNames, int>.Send(GlobalEventType.GAME_DATA_CHARACTER_ENHANCEMENT_LEVEL_CHANGED, statName, newLevel);
            return newLevel;
        }

        /// <summary>
        /// 모든 강화 능력치 레벨을 초기화합니다.
        /// </summary>
        public void ResetAllLevels()
        {
            if (EnhancementLevels != null)
            {
                EnhancementLevels.Clear();
            }
        }

        public static VCharacterEnhancement CreateDefault()
        {
            return new VCharacterEnhancement();
        }
    }
}