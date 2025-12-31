using Sirenix.OdinInspector;
using TeamSuneat;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "ExperienceConfig", menuName = "TeamSuneat/Scriptable/ExperienceConfig")]
    public class ExperienceConfigAsset : XScriptableObject
    {
        [Tooltip("초기 경험치 필요량 (레벨 1에서 레벨 2로 올라가기 위해 필요한 경험치)")]
        public int InitialExperienceRequired = 120;

        [Tooltip("경험치 증가 배율 (레벨마다 이전 레벨 대비 증가 배율)")]
        public float ExperienceGrowthRate = 1.01f;

        [Tooltip("레벨업 시 획득하는 능력치 포인트")]
        public int StatPointPerLevel = 3;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (InitialExperienceRequired <= 0)
            {
                Log.Error("초기 경험치 필요량이 0 이하입니다: {0}", name);
            }
            if (ExperienceGrowthRate <= 1.0f)
            {
                Log.Error("경험치 증가 배율이 1.0 이하입니다: {0}", name);
            }
            if (StatPointPerLevel <= 0)
            {
                Log.Error("레벨업 시 능력치 포인트가 0 이하입니다: {0}", name);
            }
#endif
        }

        /// <summary>
        /// 특정 레벨에 도달하기 위해 필요한 경험치를 계산합니다.
        /// </summary>
        /// <param name="level">목표 레벨</param>
        /// <returns>레벨 n에 도달하기 위한 총 경험치</returns>
        public int GetTotalExperienceRequired(int level)
        {
            if (level <= 1)
            {
                return InitialExperienceRequired;
            }

            // 총 경험치 = InitialExperienceRequired × (GrowthRate^level - 1) / (GrowthRate - 1)
            double growthRate = ExperienceGrowthRate;
            double numerator = InitialExperienceRequired * (System.Math.Pow(growthRate, level) - 1.0);
            double denominator = growthRate - 1.0;
            return (int)(numerator / denominator);
        }

        /// <summary>
        /// 특정 레벨에서 다음 레벨로 올라가기 위해 필요한 경험치를 계산합니다.
        /// </summary>
        /// <param name="level">현재 레벨</param>
        /// <returns>레벨 n에서 레벨 n+1로 올라가기 위해 필요한 경험치</returns>
        public int GetExperienceRequiredForNextLevel(int level)
        {
            if (level < 1)
            {
                return InitialExperienceRequired;
            }

            // 레벨 n 필요 경험치 = InitialExperienceRequired × GrowthRate^(n-1)
            return Mathf.RoundToInt(InitialExperienceRequired * Mathf.Pow(ExperienceGrowthRate, level - 1));
        }

#if UNITY_EDITOR

        public override void Rename()
        {
            PerformRename("ExperienceConfig");
        }

#endif
    }
}