using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour
    {
        private Coroutine _refreshResourceCoroutine;

        protected void StartUseAndRestoreResource()
        {
            if (AssetData == null)
            {
                return;
            }
            if (AssetData.Type != SkillType.Active)
            {
                return;
            }
            if (AssetData.ResourceConsumeType == VitalConsumeTypes.None)
            {
                return;
            }

            if (_refreshResourceCoroutine == null)
            {
                _refreshResourceCoroutine = StartXCoroutine(ProcessRefreshResourceValue());
            }
            else
            {
                LogError("스킬 독립체에서 자원 사용 또는 회복에 대한 코루틴을 중복 재생할 수 없습니다.");
            }
        }

        protected void StopUseAndRestoreResource()
        {
            StopXCoroutine(ref _refreshResourceCoroutine);
        }

        protected IEnumerator ProcessRefreshResourceValue()
        {
            yield return null;

            if (DetermineUseResourceValue())
            {
                if (CheckNoCostResource())
                {
                    // No Cost
                }
                else if (!TryUseVitalResource())
                {
                    // 자원 부족 시 처리
                    LogWarning("스킬 사용에 필요한 자원이 부족합니다.");
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
            if (AssetData == null || AssetData.UseResourceValue <= 0f)
            {
                return false;
            }

            return true;
        }

        private bool DetermineRestoreResourceValue()
        {
            if (AssetData == null || AssetData.RestoreResourceValue <= 0f)
            {
                return false;
            }

            return true;
        }

        protected bool TryUseVitalResource()
        {
            if (Owner == null || !Owner.IsAlive)
            {
                // 캐릭터가 사망했다면 전투 자원(생명력, 마나, 광기)을 사용할 수 없습니다.
                return false;
            }

            if (AssetData == null || Owner.MyVital == null)
            {
                return false;
            }

            float useResourceValue = AssetData.UseResourceValue;
            if (useResourceValue > 0f)
            {
                if (AssetData.ForceResourceConsume)
                {
                    LogInfo("스킬 독립체에서 자원을 소모합니다. {0}, {1}", AssetData.ResourceConsumeType, useResourceValue);
                    Owner.MyVital.UseCurrentValue(AssetData.ResourceConsumeType, useResourceValue);
                    return true;
                }
                else if (Owner.MyVital.GetCurrent(AssetData.ResourceConsumeType) >= useResourceValue)
                {
                    LogInfo("스킬 독립체에서 자원을 소모합니다. {0}, {1}", AssetData.ResourceConsumeType, useResourceValue);
                    Owner.MyVital.UseCurrentValue(AssetData.ResourceConsumeType, useResourceValue);
                    return true;
                }
            }

            return false;
        }

        private bool CheckNoCostResource()
        {
            return false;
        }

        protected bool TryRestoreVitalResource()
        {
            if (AssetData == null || Owner == null || Owner.MyVital == null)
            {
                return false;
            }

            // Resource 타입일 때는 게이지 증가량(0~1)으로 처리
            if (AssetData.ResourceConsumeType == VitalConsumeTypes.FixedResource)
            {
                if (Owner.MyVital.Mana != null)
                {
                    float gainAmount = Mathf.Clamp01(AssetData.RestoreResourceValue);
                    if (gainAmount > 0f)
                    {
                        LogInfo("스킬 독립체에서 마나 게이지를 증가시킵니다. {0}, {1:F2}", AssetData.ResourceConsumeType, gainAmount);
                        Owner.MyVital.Mana.OnAttackSuccess(gainAmount);
                        return true;
                    }
                }
                return false;
            }

            // Pulse 타입일 때는 게이지 증가량(0~1)으로 처리
            if (AssetData.ResourceConsumeType == VitalConsumeTypes.FixedPulse)
            {
                if (Owner.MyVital.Pulse != null)
                {
                    float gainAmount = Mathf.Clamp01(AssetData.RestoreResourceValue);
                    if (gainAmount > 0f)
                    {
                        LogInfo("스킬 독립체에서 펄스 게이지를 증가시킵니다. {0}, {1:F2}", AssetData.ResourceConsumeType, gainAmount);
                        Owner.MyVital.Pulse.OnAttackSuccess(gainAmount);
                        return true;
                    }
                }
                return false;
            }

            // Life나 Barrier는 기존 방식 유지
            float value = AssetData.RestoreResourceValue;
            if (value > 0f)
            {
                LogInfo("스킬 독립체에서 자원을 회복합니다. {0}, {1}", AssetData.ResourceConsumeType, value);
                Owner.MyVital.AddCurrentValue(AssetData.ResourceConsumeType, value);

                return true;
            }

            return false;
        }
    }
}