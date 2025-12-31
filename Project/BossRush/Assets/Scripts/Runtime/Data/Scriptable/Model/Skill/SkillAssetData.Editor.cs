using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class SkillAssetData
    {
        private void CustomLog()
        {
#if UNITY_EDITOR
            if (MaxLevel == 0)
            {
                Log.Error("스킬의 최대 레벨이 설정되지 않았습니다: {0}", Name);
            }
            if (CooldownType == CooldownTypes.TimeBased && BaseCooldownTime <= 0)
            {
                Log.Error("시간 기반 쿨타임인데 기본 대기 시간이 설정되지 않았습니다: {0}", Name);
            }
            if (CooldownType == CooldownTypes.AttackCountBased && BaseCooldownAttackCount <= 0)
            {
                Log.Error("공격수 기반 쿨타임인데 기본 필요 공격수가 설정되지 않았습니다: {0}", Name);
            }
#endif
        }

#if UNITY_EDITOR

        #region Inspector Color Methods

        protected Color GetIntArrayColor(int[] array)
        {
            if (array == null || array.Length == 0)
            {
                return GameColors.DarkGray;
            }
            return GameColors.GreenYellow;
        }

        protected Color GetSkillAttributeColor(SkillAttributeTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetSkillTypeColor(SkillTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetGradeColor(GradeNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetCooldownTypeColor(CooldownTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetIntArrayColor()
        {
            if (LevelUpCosts == null || LevelUpCosts.Length == 0)
            {
                return GameColors.DarkGray;
            }
            return GameColors.GreenYellow;
        }

        #endregion Inspector Color Methods

#endif
    }
}