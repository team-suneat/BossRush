using Lean.Pool;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        private int _skillLevel;
        private int _additionalLevelByStat;
        private int _maxLevel;

        public int Level => _skillLevel;

        private void SetLevel(int level)
        {
            _skillLevel = level;
            LogInfo("스킬 엔티티의 레벨({0})을 설정합니다. ", Level.ToSelectString(1));
        }

        public void ResetLevel()
        {
            _skillLevel = 0;
            LogProgress("스킬 엔티티의 레벨({0})을 초기화합니다. ", Level);
        }

        //

        public void ApplyAdditionalLevelByStat(int skillLevel)
        {
            if (_skillData.UseAddLevel)
            {
                _additionalLevelByStat = 0;
                _additionalLevelByStat += LoadAddAllSkillLevel(Name, _additionalLevelByStat);
                _additionalLevelByStat += LoadAddAssignedSkillLevel(Name, _additionalLevelByStat);
                _additionalLevelByStat += LoadAddAwardedSkillLevel(Name, _additionalLevelByStat);
                _additionalLevelByStat += LoadAddCategorySkillLevel(_skillData, _additionalLevelByStat);

                int totalLevel = skillLevel + _additionalLevelByStat;
                _maxLevel = LoadMaxCategorySkillLevel(_skillData, totalLevel);
                if (_maxLevel > 0)
                {
                    _skillLevel = Mathf.Min(totalLevel, _maxLevel);
                    LogInfo("스킬 엔티티의 레벨({0}={1}+{2}/{3})을 설정합니다. ", totalLevel.ToSelectString(1), skillLevel, _additionalLevelByStat, _maxLevel);
                }
                else
                {
                    _skillLevel = totalLevel;
                    LogInfo("스킬 엔티티의 레벨({0}={1}+{2})을 설정합니다. ", totalLevel.ToSelectString(1), skillLevel, _additionalLevelByStat);
                }
            }
            else
            {
                SetLevel(skillLevel);
            }
        }

        private int LoadAddAllSkillLevel(SkillNames skillName, int skillLevel)
        {
            int additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAllSkillLevel);
            if (additionalLevel > 0)
            {
                LogProgress("스킬({0})의 레벨({1})에 '모든 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillName.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                return additionalLevel;
            }

            return 0;
        }

        private int LoadAddAwardedSkillLevel(SkillNames skillName, int skillLevel)
        {
            VSkill skillInfo = CharacterInfo.Skill.Find(skillName);
            if (skillInfo != null)
            {
                if (skillInfo.Level > 0)
                {
                    // 스킬이 배운 상태라면 스탯 효과를 적용합니다.
                    int additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAwardedSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '배운 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillName.ToLogString(), skillLevel, additionalLevel.ToSelectString());

                        return additionalLevel;
                    }
                }
            }

            return 0;
        }

        private int LoadAddAssignedSkillLevel(SkillNames skillName, int skillLevel)
        {
            VSkill skillInfo = CharacterInfo.Skill.Find(skillName);
            if (skillInfo != null)
            {
                if (skillInfo.ActionName != ActionNames.None)
                {
                    // 할당된 스킬이라면 스탯 효과를 적용합니다.
                    int additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAssignSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '할당된 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillName.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                }
            }

            return 0;
        }

        private int LoadAddCategorySkillLevel(SkillAssetData skillData, int skillLevel)
        {
            int additionalLevel;
            switch (skillData.Category)
            {
                case SkillCategories.Basic:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddBasicSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '기본 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Core:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddCoreSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '핵심 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Assistant:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAssistantSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '보조 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Power:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddPowerSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '강화 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Ultimate:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddUltimateSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '궁극 스킬 레벨 증가' 스탯 효과(+{2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;
            }

            return 0;
        }

        private int LoadMaxCategorySkillLevel(SkillAssetData skillData, int skillLevel)
        {
            int skillMaxLevel;
            switch (skillData.Category)
            {
                case SkillCategories.Basic:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxBasicSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '기본 스킬 최대 레벨' 스탯 효과({2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Core:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxCoreSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '핵심 스킬 최대 레벨' 스탯 효과({2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Assistant:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxAssistantSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '보조 스킬 최대 레벨' 스탯 효과({2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Power:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxPowerSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '강화 스킬 최대 레벨' 스탯 효과({2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Ultimate:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxUltimateSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("스킬({0})의 레벨({1})에 '궁극 스킬 최대 레벨' 스탯 효과({2})를 적용합니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;
            }

            return 0;
        }
    }
}