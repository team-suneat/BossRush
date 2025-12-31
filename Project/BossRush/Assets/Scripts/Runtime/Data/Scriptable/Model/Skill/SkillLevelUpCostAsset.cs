using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "SkillLevelUpCost", menuName = "TeamSuneat/Scriptable/SkillLevelUpCost")]
    public class SkillLevelUpCostAsset : XScriptableObject
    {
        [FoldoutGroup("#ActiveSkillCost")]
        [InfoBox("액티브 스킬 레벨업 비용 테이블입니다. 인덱스 0 = 레벨 1 비용")]
        [SuffixLabel("에메랄드")]
        public int[] ActiveSkillCosts = new int[220]; // 액티브 스킬 최대 레벨: 220

        [FoldoutGroup("#PassiveSkillCost")]
        [InfoBox("패시브 스킬 레벨업 비용 테이블입니다. 인덱스 0 = 레벨 1 비용")]
        [SuffixLabel("에메랄드")]
        public int[] PassiveSkillCosts = new int[20]; // 패시브 스킬 최대 레벨: 20

        [FoldoutGroup("#OtherSkillCost")]
        [InfoBox("기타 스킬 레벨업 비용 테이블입니다. 인덱스 0 = 레벨 1 비용")]
        [SuffixLabel("에메랄드")]
        public int[] OtherSkillCosts = new int[10]; // 기타 스킬 최대 레벨: 10

        public override void OnLoadData()
        {
            base.OnLoadData();
            Validate();
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (ActiveSkillCosts == null || ActiveSkillCosts.Length == 0)
            {
                Log.Warning(LogTags.ScriptableData, "[SkillLevelUpCost] 액티브 스킬 비용 배열이 비어있습니다: {0}", name);
            }

            if (PassiveSkillCosts == null || PassiveSkillCosts.Length == 0)
            {
                Log.Warning(LogTags.ScriptableData, "[SkillLevelUpCost] 패시브 스킬 비용 배열이 비어있습니다: {0}", name);
            }

            if (OtherSkillCosts == null || OtherSkillCosts.Length == 0)
            {
                Log.Warning(LogTags.ScriptableData, "[SkillLevelUpCost] 기타 스킬 비용 배열이 비어있습니다: {0}", name);
            }
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("SkillLevelUpCost");
        }

        /// <summary>
        /// 스킬 타입과 레벨에 따른 레벨업 비용을 반환합니다.
        /// </summary>
        public int GetLevelUpCost(SkillTypes skillType, int level)
        {
            if (level <= 0)
            {
                return 0;
            }

            int[] costArray = skillType switch
            {
                SkillTypes.Active => ActiveSkillCosts,
                SkillTypes.Passive => PassiveSkillCosts,
                SkillTypes.Other => OtherSkillCosts,
                _ => null
            };

            if (costArray == null || level > costArray.Length)
            {
                return 0;
            }

            // 레벨 1부터 시작하므로 인덱스는 level - 1
            return costArray[level - 1];
        }

        [FoldoutGroup("#Button")]
        [Button("기본값 설정", ButtonSizes.Medium)]
        public void SetDefaultValues()
        {
            // 액티브 스킬 비용 설정 (레벨 1: 100, 이후 레벨당 10% 증가)
            if (ActiveSkillCosts == null || ActiveSkillCosts.Length == 0)
            {
                ActiveSkillCosts = new int[220];
            }

            for (int i = 0; i < ActiveSkillCosts.Length; i++)
            {
                // 레벨 1: 100, 레벨 2: 110, 레벨 3: 121... (10% 증가)
                ActiveSkillCosts[i] = (int)(100 * System.Math.Pow(1.1, i));
            }

            // 패시브 스킬 비용 설정 (레벨 1: 50, 이후 레벨당 15% 증가)
            if (PassiveSkillCosts == null || PassiveSkillCosts.Length == 0)
            {
                PassiveSkillCosts = new int[20];
            }

            for (int i = 0; i < PassiveSkillCosts.Length; i++)
            {
                // 레벨 1: 50, 레벨 2: 58, 레벨 3: 67... (15% 증가)
                PassiveSkillCosts[i] = (int)(50 * System.Math.Pow(1.15, i));
            }

            // 기타 스킬 비용 설정 (레벨 1: 200, 이후 레벨당 20% 증가)
            if (OtherSkillCosts == null || OtherSkillCosts.Length == 0)
            {
                OtherSkillCosts = new int[10];
            }

            for (int i = 0; i < OtherSkillCosts.Length; i++)
            {
                // 레벨 1: 200, 레벨 2: 240, 레벨 3: 288... (20% 증가)
                OtherSkillCosts[i] = (int)(200 * System.Math.Pow(1.2, i));
            }

            EditorUtility.SetDirty(this);
            Log.Info(LogTags.ScriptableData, "[SkillLevelUpCost] 기본값이 설정되었습니다: {0}", name);
        }

#endif
    }
}