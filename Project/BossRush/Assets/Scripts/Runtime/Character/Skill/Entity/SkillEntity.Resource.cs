using Lean.Pool;
using TeamSuneat.Data;
using TeamSuneat.Setting;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        private VitalConsumeTypes GetEffectiveVitalType()
        {
            // VitalConsumeType이 None이고 ResourceCostPerCooldownSecond 능력치가 있으면 Resource 타입 사용
            if (_skillData.VitalConsumeType == VitalConsumeTypes.None)
            {
                if (!ShouldUseCooldownTime())
                {
                    return VitalConsumeTypes.FixedResource;
                }
                else if (_skillData.Category == SkillCategories.Basic)
                {
                    if (Owner.Stat.ContainsKey(StatNames.FixedBasicSkillResourceCost))
                    {
                        return VitalConsumeTypes.FixedResource;
                    }
                }
            }

            return _skillData.VitalConsumeType;
        }

        public bool CheckResourceCost()
        {
            VitalConsumeTypes effectiveType = VitalConsumeTypes.None;
            float currentValue = 0;
            int resultValue = 0;

            // 치트나 NoResourceCost 능력치 체크
            if (Owner != null && Owner.IsPlayer)
            {
                if (GameSetting.Instance.Cheat.NotCostResoure)
                {
                    if (Log.LevelProgress)
                    {
                        Log.Progress(LogTags.Skill_Cost, FormatEntityLog("치트에 의해 기술 시전 시 자원을 소모하지 않습니다."));
                    }
                    return true;
                }

                if (_skillData.Category == SkillCategories.Core)
                {
                    if (Owner.Stat.ContainsKey(StatNames.NoResourceCostForCoreSkill))
                    {
                        if (Log.LevelProgress)
                        {
                            Log.Progress(LogTags.Skill_Cost, FormatEntityLog("능력치에 의해 핵심 기술 시전 시 자원을 소모하지 않습니다."));
                        }
                        return true;
                    }
                }
            }

            effectiveType = GetEffectiveVitalType();
            if (effectiveType == VitalConsumeTypes.None) { return true; }
            if (_skillData.MinResourceCost > 0) { return true; }

            currentValue = Owner.MyVital.GetCurrent(effectiveType);
            resultValue = Owner.MyVital.CalculateResourceCostByStat(ResourceCost, _skillData);
            return currentValue >= resultValue;
        }

        //

        /// <summary> 기술 활성화 시 자원을 소모합니다. </summary>
        public void UseVitalResourceOnActivate()
        {
            if (_skillCount != 0 && _skillMaxCount > 1)
            {
                LogInfoSkillNotConsumedResource(_skillCount, _skillMaxCount);
                return;
            }

            if (_skillData.Category == SkillCategories.Core && Owner.Stat.ContainsKey(StatNames.NoResourceCostForCoreSkill))
            {
                if (Log.LevelProgress)
                {
                    Log.Progress(LogTags.Skill_Cost, FormatEntityLog("능력치에 의해 핵심 기술 시전 시 자원을 소모하지 않습니다."));
                }
                return;
            }

            int resourceCost;
            resourceCost = CalculateResourceCostUsed();
            if (resourceCost > 0)
            {
                VitalConsumeTypes effectiveType = GetEffectiveVitalType();
                Owner.MyVital.Use(effectiveType, _skillData, resourceCost);
            }
            else
            {
                if (Log.LevelProgress)
                {
                    Log.Progress(LogTags.Skill_Cost, FormatEntityLog("기술 활성화 시 자원을 사용하지 않는 기술입니다."));
                }
            }
        }

        //

        /// <summary> 사용하는 자원 비용을 계산합니다. </summary>
        private int CalculateResourceCostUsed()
        {
            int resourceCost = ResourceCost;

            if (TryNoCostPassive())
            {
                resourceCost = 0;
            }
            else
            {
                if (_skillData.MinResourceCost > 0)
                {
                    int currentValue = Owner.MyVital.GetCurrent(_skillData.VitalConsumeType);
                    if (currentValue > _skillData.MinResourceCost)
                    {
                        if (currentValue < ResourceCost)
                        {
                            resourceCost = currentValue - _skillData.MinResourceCost;
                            LogProgressResourceCostAdjusted(currentValue, ResourceCost, _skillData.MinResourceCost, resourceCost);
                        }
                        else
                        {
                            LogProgressResourceCostNormal(currentValue, ResourceCost);
                        }
                    }
                    else
                    {
                        LogProgressResourceCostTooLow(currentValue, _skillData.MinResourceCost);
                    }
                }
            }

            return resourceCost;
        }

        /// <summary> 패시브를 통해 기술의 자원을 소모하지 않습니다. </summary>
        private bool TryNoCostPassive()
        {
            if (_skillData.Category == SkillCategories.Core)
            {
                float coreSkillNoCost = Owner.Stat.FindValueOrDefault(StatNames.CoreSkillReturnCost);

                if (TSRandomEx.GetFloatValue() < coreSkillNoCost)
                {
                    LogProgressCoreSkillNoCost(coreSkillNoCost);
                    return true;
                }
            }

            return false;
        }

        /// <summary> 자원 소모량을 갱신합니다. </summary>
        private void RefreshResourceCost()
        {
            if (_skillData.VitalConsumeType != VitalConsumeTypes.None)
            {
                SetResourceCostByVitalType();
                return;
            }

            if (_skillData.Category == SkillCategories.Basic)
            {
                SetResourceCostForBasic();
                return;
            }

            if (_skillData.Category == SkillCategories.Power)
            {
                SetResourceCostForPower();
                return;
            }

            // 정의되지 않은 경우, 자원 소모 없음
            ResourceCost = 0;
            LogProgressNoResourceCost();
        }

        private void SetResourceCostByVitalType()
        {
            if (!Owner.MyVital.IsAlive)
            {
                LogWarningCharacterNotAlive(_skillData.VitalConsumeType, ResourceCost);
                return;
            }

            int resourceDefaultCost = _skillData.GetResourceCost(Owner.MyVital, Level);
            if (_skillData.Category == SkillCategories.Core)
            {
                if (_skillData.Name == SkillNames.LeapSlash && Owner.Stat.ContainsKey(StatNames.FixedLeapSlashResourceCost))
                {
                    int addResourceCost = Owner.Stat.FindValueOrDefaultToInt(StatNames.FixedLeapSlashResourceCost);
                    ResourceCost = resourceDefaultCost + addResourceCost;
                    LogProgressResourceCostSet(VitalConsumeTypes.FixedResource, ResourceCost);
                }

                if (Owner.Stat.ContainsKey(StatNames.CoreSkillResourceCostRate))
                {
                    float multiplier = CalculateCostMultiplier();
                    ResourceCost = Mathf.RoundToInt(multiplier * resourceDefaultCost);
                    LogProgressResourceCostSet(_skillData.VitalConsumeType, ResourceCost);
                }

                if (ResourceCost == 0)
                {
                    ResourceCost = resourceDefaultCost;
                }
            }
            else
            {
                ResourceCost = resourceDefaultCost;
                LogProgressResourceCostSet(_skillData.VitalConsumeType, ResourceCost);
            }
        }

        private void SetResourceCostForBasic()
        {
            if (Owner.Stat.ContainsKey(StatNames.FixedBasicSkillResourceCost))
            {
                float statValue = Owner.Stat.FindCalculateValue(StatNames.FixedBasicSkillResourceCost);
                ResourceCost = Mathf.RoundToInt(statValue);
                LogProgressResourceCostSet(VitalConsumeTypes.FixedResource, ResourceCost);
            }
            else
            {
                ResourceCost = 0;
                LogProgressNoResourceCost();
            }
        }

        private void SetResourceCostForPower()
        {
            ResourceCost = 0;

            if (_skillData.Name == SkillNames.WolfBrawl)
            {
                if (Owner.Stat.ContainsKey(StatNames.ResourceCostForWolfBrawl))
                {
                    ResourceCost = Owner.Stat.FindValueOrDefaultToInt(StatNames.ResourceCostForWolfBrawl);
                    LogProgressResourceCostSet(VitalConsumeTypes.FixedResource, ResourceCost);
                }
            }

            if (Owner.Stat.ContainsKey(StatNames.PowerSkillResourceCostPerCooldown))
            {
                float costRatio = Owner.Stat.FindValueOrDefault(StatNames.PowerSkillResourceCostPerCooldown);
                if (!costRatio.IsZero())
                {
                    ResourceCost += Mathf.RoundToInt(costRatio * CooldownTime);
                    LogProgressResourceCostByCooldownSet(CooldownTime, costRatio, ResourceCost);
                }
            }

            if (ResourceCost == 0)
            {
                LogProgressNoResourceCost();
            }
        }

        private float CalculateCostMultiplier()
        {
            float multiplier = Owner.Stat.FindValueOrDefault(StatNames.SkillResourceCostRate);
            if (_skillData.IsValid())
            {
                if (_skillData.Category == SkillCategories.Core)
                {
                    float addMultiplier = Owner.Stat.FindValueOrDefault(StatNames.CoreSkillResourceCostRate);
                    if (!addMultiplier.IsZero())
                    {
                        multiplier += addMultiplier;
                    }
                }
            }

            StatData multiplierData = JsonDataManager.FindStatDataClone(StatNames.SkillResourceCostRate);
            return Mathf.Clamp(multiplier, multiplierData.MinValue, multiplierData.MaxValue);
        }

        #region Log

        private void LogInfoSkillNotConsumedResource(int skillCount, int skillMaxCount)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("기술 횟수가 0 이상이면 자원을 소모하지 않습니다. {0}/{1}", skillCount, skillMaxCount);
                Log.Progress(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogProgressResourceCostAdjusted(int currentValue, int resourceCost, int minResourceCost, int adjustedCost)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("현재값({0})이 자원소모값({1})보다 작다면 현재값({0})과 최소자원값({2})의 차이({3})만큼 자원을 소모합니다.",
                    currentValue, resourceCost, minResourceCost, adjustedCost);
                Log.Progress(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogProgressResourceCostNormal(int currentValue, int resourceCost)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("현재값({0})이 자원소모값({1})보다 크거나 같다면 자원소모값({1})만큼 자원을 소모합니다.", currentValue, resourceCost);
                Log.Progress(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogProgressResourceCostTooLow(int currentValue, int minResourceCost)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("현재값({0})이 최소자원값({1})보다 작을 때 자원을 소모하지 않습니다.", currentValue, minResourceCost);
                Log.Progress(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogProgressCoreSkillNoCost(float coreSkillNoCost)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("핵심 기술의 자원이 소모되지 않습니다 확률 : {0}", coreSkillNoCost);

                Log.Progress(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogProgressResourceCostByCooldownSet(float cooldown, float costRatio, int resourceCost)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("기술의 소모 자원의 소모량({0})을 재사용 대기시간({1} * {2})에 비례하여 설정합니다.", resourceCost, cooldown, costRatio);
                Log.Progress(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogProgressResourceCostSet(VitalConsumeTypes vitalConsumeType, int resourceCost)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("자원({0})의 소모량({1})을 설정합니다.", vitalConsumeType, resourceCost);
                Log.Progress(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogWarningCharacterNotAlive(VitalConsumeTypes vitalConsumeType, int resourceCost)
        {
            if (Log.LevelWarning)
            {
                string content = string.Format("시전 캐릭터가 살아있지 않습니다. 자원({0})의 소모량({1})을 설정할 수 없습니다.", vitalConsumeType, resourceCost);
                Log.Warning(LogTags.Skill_Cost, FormatEntityLog(content));
            }
        }

        private void LogProgressNoResourceCost()
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Skill_Cost, FormatEntityLog("이 기술은 자원을 소모하지 않습니다."));
            }
        }

        #endregion Log
    }
}