using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterGrowth
    {
        /// <summary>
        /// 능력치 포인트 (레벨업 시 획득)
        /// </summary>
        public int StatPoint = 0;

        /// <summary>
        /// 성장 능력치별 레벨 (string -> Level)
        /// </summary>
        public Dictionary<string, int> GrowthLevels = new();

        public void OnLoadGameData()
        {
            GrowthLevels ??= new Dictionary<string, int>();

            Log.Info(LogTags.GameData_Character, "[Character] 성장 능력치 레벨 데이터를 불러옵니다. 총 {0}개, 능력치 포인트: {1}",
                GrowthLevels.Count, StatPoint);
        }

        public int GetLevel(CharacterGrowthTypes growthType)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                return 0;
            }

            if (GrowthLevels == null)
            {
                return 0;
            }

            string key = growthType.ToString();
            return GrowthLevels.TryGetValue(key, out int level) ? level : 0;
        }

        private void SetLevel(CharacterGrowthTypes growthType, int level)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                Log.Warning(LogTags.GameData_Character, "잘못된 성장 타입입니다: {0}", growthType);
                return;
            }

            GrowthLevels ??= new Dictionary<string, int>();

            string key = growthType.ToString();
            GrowthLevels[key] = level;

            Log.Info(LogTags.GameData_Character, "성장 능력치 {0}의 레벨을 {1}로 설정합니다.", growthType, level);

            GlobalEvent<CharacterGrowthTypes, int>.Send(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED, growthType, level);
        }

        public void AddLevel(CharacterGrowthTypes growthType, int addLevel = 1)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                Log.Warning(LogTags.GameData_Character, "잘못된 성장 타입입니다: {0}", growthType);
                return;
            }

            int currentLevel = GetLevel(growthType);
            int newLevel = currentLevel + addLevel;
            SetLevel(growthType, newLevel);
        }

        //

        public int AddStatPoint(int addPoint)
        {
            StatPoint += addPoint;
            Log.Info(LogTags.GameData_Character, "능력치 포인트를 {0} 추가합니다. 총 능력치 포인트: {1}", addPoint, StatPoint);
            GlobalEvent<int>.Send(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_STAT_POINT_CHANGED, StatPoint);
            return StatPoint;
        }

        public bool CanConsumeStatPoint(int consumePoint)
        {
            return StatPoint >= consumePoint;
        }

        public bool CanConsumeStatPointOrNotify(int consumePoint)
        {
            bool canConsume = CanConsumeStatPoint(consumePoint);
            if (!canConsume)
            {
                GlobalEvent.Send(GlobalEventType.STAT_POINT_SHORTAGE);
            }
            return canConsume;
        }

        public bool ConsumeStatPoint(int consumePoint)
        {
            if (StatPoint < consumePoint)
            {
                Log.Warning(LogTags.GameData_Character, "능력치 포인트가 부족합니다. 현재: {0}, 필요: {1}", StatPoint, consumePoint);
                return false;
            }

            StatPoint -= consumePoint;
            Log.Info(LogTags.GameData_Character, "능력치 포인트를 {0} 소비합니다. 남은 능력치 포인트: {1}", consumePoint, StatPoint);

            GlobalEvent<int>.Send(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_STAT_POINT_CHANGED, StatPoint);
            return true;
        }

        //

        public void Clear()
        {
            GrowthLevels?.Clear();
            StatPoint = 0;
        }

        public int GetTotalConsumedStatPoints()
        {
            int total = 0;
            if (GrowthLevels != null)
            {
                foreach (var level in GrowthLevels.Values)
                {
                    total += level;
                }
            }
            return total;
        }

        public int ResetGrowthLevels()
        {
            int totalConsumed = GetTotalConsumedStatPoints();
            GrowthLevels?.Clear();
            Log.Info(LogTags.GameData_Character, "성장 능력치 레벨을 초기화합니다. 반환될 능력치 포인트: {0}", totalConsumed);
            return totalConsumed;
        }

        public static VCharacterGrowth CreateDefault()
        {
            return new VCharacterGrowth
            {
                StatPoint = 0
            };
        }
    }
}