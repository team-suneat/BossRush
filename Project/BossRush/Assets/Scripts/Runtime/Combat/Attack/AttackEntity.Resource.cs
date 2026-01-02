using System.Collections;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        private Coroutine _refreshResourceCoroutine;

        protected void StartUseAndRestoreResource()
        {
            if (_refreshResourceCoroutine == null)
            {
                _refreshResourceCoroutine = StartXCoroutine(ProcessRefreshResourceValue());
            }
            else
            {
                LogError("공격 독립체에서 자원 사용 또는 회복에 대한 코루틴을 중복 재생할 수 없습니다.");
            }
        }

        protected void StopUseAndRestoreResource()
        {
            StopXCoroutine(ref _refreshResourceCoroutine);
        }

        protected IEnumerator ProcessRefreshResourceValue()
        {
            if (AssetData.ConsumeDealyTime > 0)
            {
                yield return new WaitForSeconds(AssetData.ConsumeDealyTime);
            }

            if (DetermineUseResourceValue())
            {
                if (CheckNoCostResource())
                {
                    // No Cost
                }
                else if (!TryUseVitalResource())
                {
                    if (AssetData.StopAttackAnimationOnResourceLack)
                    {
                        Owner.CharacterAnimator.StopSequenceSkill(AssetData.SkillName);
                    }
                }
            }

            if (DetermineRestoreResourceValue())
            {
                TryRestoreVitalResource();
            }

            _refreshResourceCoroutine = null;
        }

        private bool DetermineUseResourceValue()
        {
            if (AssetData.UseResourceValue.IsZero())
            {
                return false;
            }

            return true;
        }

        private bool DetermineRestoreResourceValue()
        {
            if (AssetData.RestoreResourceValue.IsZero())
            {
                return false;
            }

            return true;
        }

        protected bool TryUseVitalResource()
        {
            if (!Owner.IsAlive)
            {
                // 캐릭터가 사망했다면 전투 자원(생명력, 마나, 광기)을 사용할 수 없습니다.
                return false;
            }

            int useResourceValue = 0;
            if (AssetData.ResourceConsumeType.IsPercentMax())
            {
                float maxValue = Owner.MyVital.GetMax(AssetData.ResourceConsumeType);
                if (maxValue > 0)
                {
                    useResourceValue = Mathf.RoundToInt(AssetData.UseResourceValue * maxValue);
                }
            }
            else
            {
                useResourceValue = Mathf.RoundToInt(AssetData.UseResourceValue);
            }

            if (useResourceValue > 0)
            {
                if (AssetData.ForceResourceConsume)
                {
                    LogInfo("공격 독립체에서 자원을 소모합니다. {0}, {1}", AssetData.ResourceConsumeType, useResourceValue);
                    Owner.MyVital.UseCurrentValue(AssetData, useResourceValue);
                    return true;
                }
                else if (Owner.MyVital.GetCurrent(AssetData.ResourceConsumeType) >= useResourceValue)
                {
                    LogInfo("공격 독립체에서 자원을 소모합니다. {0}, {1}", AssetData.ResourceConsumeType, useResourceValue);
                    Owner.MyVital.UseCurrentValue(AssetData, useResourceValue);
                    return true;
                }
            }

            return false;
        }

        private bool CheckNoCostResource()
        {
            if (!SkillAssetData.IsValid()) { return false; }

            switch (SkillAssetData.Category)
            {
                case SkillCategories.Core:
                    {
                        if (Owner.Stat.ContainsKey(StatNames.NoResourceCostForCoreSkill))
                        {
                            LogInfo("핵심 기술의 자원이 소모되지 않습니다.");
                            return true;
                        }

                        float coreSkillNoCost = Owner.Stat.FindValueOrDefault(StatNames.CoreSkillReturnCost);
                        if (TSRandomEx.GetFloatValue() < coreSkillNoCost)
                        {
                            LogInfo("핵심 기술의 자원이 소모되지 않습니다 확률 : {0}", coreSkillNoCost);
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        protected bool TryRestoreVitalResource()
        {
            int value = 0;
            if (AssetData.ResourceConsumeType.IsPercentMax())
            {
                int maxValue = Owner.MyVital.GetMax(AssetData.ResourceConsumeType);

                if (maxValue > 0)
                {
                    value = Mathf.RoundToInt(AssetData.RestoreResourceValue * maxValue);
                }
            }
            else if (AssetData.ResourceConsumeType.IsPercentCurrent())
            {
                int currentValue = Owner.MyVital.GetCurrent(AssetData.ResourceConsumeType);
                if (currentValue > 0)
                {
                    value = Mathf.RoundToInt(AssetData.RestoreResourceValue * currentValue);
                }
            }
            else
            {
                value = (int)AssetData.RestoreResourceValue;
            }

            if (value > 0)
            {
                ApplyRestoreDoubleForBasicSkill(ref value);
                LogInfo("공격독립체에서 자원을 회복합니다. {0}, {1}", AssetData.ResourceConsumeType, value);
                Owner.MyVital.AddCurrentValue(AssetData.ResourceConsumeType, value);

                return true;
            }

            return false;
        }

        private void ApplyRestoreDoubleForBasicSkill(ref int restoreValue)
        {
            SkillNames skillName;
            if (AssetData.LinkedSkillNameForLevel != SkillNames.None)
            {
                skillName = AssetData.LinkedSkillNameForLevel;
            }
            else if (AssetData.SkillName != SkillNames.None)
            {
                skillName = AssetData.SkillName;
            }
            else
            {
                return;
            }

            SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(skillName);
            if (skillData.Category == SkillCategories.Basic)
            {
                // 적중 시 고정 자원 회복 (정수)
                if (Owner.Stat.ContainsKey(StatNames.ResourceRecoveryOnHit))
                {
                    int statValue = Owner.Stat.FindValueOrDefaultToInt(StatNames.ResourceRecoveryOnHit);
                    restoreValue += statValue;
                    LogRestoreAddResourceForBasic(statValue);
                }

                // 적중 시 자원 회복 배율 (소수, 기본값: 100%)
                float multiplier = Owner.Stat.FindValueOrDefault(StatNames.ResourceRecoveryOnHitRate);
                if (multiplier > 1f)
                {
                    LogRestoreAddResourceRateForBasic(multiplier);
                }

                if (Owner.Stat.ContainsKey(StatNames.DoubleResourceRecoveryOnHit))
                {
                    float chance = Owner.Stat.FindValueOrDefault(StatNames.DoubleResourceRecoveryOnHit);
                    if (TSRandomEx.GetFloatValue() < chance)
                    {
                        multiplier += 1;
                        LogRestoreAddResourceRateForBasic(1, chance);
                    }
                }

                LogRestoreResourceForBasic(multiplier, restoreValue);
                restoreValue = Mathf.RoundToInt(restoreValue * multiplier);
            }
        }

        private void LogRestoreAddResourceForBasic(int fixedValue)
        {
            if (Log.LevelProgress)
                LogProgress("기본 기술로 얻는 자원을 {0} 더 얻습니다. ", ValueStringEx.GetValueString(fixedValue, true));
        }

        private void LogRestoreAddResourceRateForBasic(float multiplier)
        {
            if (Log.LevelProgress)
                LogProgress("기본 기술로 얻는 자원을 {0} 더 얻습니다. ", ValueStringEx.GetValueString(multiplier, true));
        }

        private void LogRestoreAddResourceRateForBasic(float multiplier, float chance)
        {
            if (Log.LevelProgress)
                LogProgress("기본 기술로 얻는 자원을 {0} 더 얻습니다. 확률: {1}", ValueStringEx.GetPercentString(1, true), ValueStringEx.GetPercentString(chance, true));
        }

        private void LogRestoreResourceForBasic(float multiplier, float restoreValue)
        {
            if (Log.LevelProgress)
                LogProgress("기본 기술로 얻는 자원: {0} * {1} = {2} ",
                    restoreValue,
                    ValueStringEx.GetPercentString(1, true),
                    ValueStringEx.GetValueString(Mathf.RoundToInt(restoreValue * multiplier), true));
        }
    }
}