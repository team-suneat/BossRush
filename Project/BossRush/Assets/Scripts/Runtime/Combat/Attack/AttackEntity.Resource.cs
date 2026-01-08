using System.Collections;

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

            int useResourceValue = Mathf.RoundToInt(AssetData.UseResourceValue);

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
            return false;
        }

        protected bool TryRestoreVitalResource()
        {
            // Resource 타입일 때는 게이지 증가량(0~1)으로 처리
            if (AssetData.ResourceConsumeType == VitalConsumeTypes.FixedResource)
            {
                if (Owner.MyVital.Mana != null)
                {
                    float gainAmount = Mathf.Clamp01(AssetData.RestoreResourceValue);
                    if (gainAmount > 0f)
                    {
                        LogInfo("공격독립체에서 마나 게이지를 증가시킵니다. {0}, {1:F2}", AssetData.ResourceConsumeType, gainAmount);
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
                        LogInfo("공격독립체에서 펄스 게이지를 증가시킵니다. {0}, {1:F2}", AssetData.ResourceConsumeType, gainAmount);
                        Owner.MyVital.Pulse.OnAttackSuccess(gainAmount);
                        return true;
                    }
                }
                return false;
            }

            // FixedResourceAndPulse 타입일 때는 마나와 펄스를 동시에 회복
            if (AssetData.ResourceConsumeType == VitalConsumeTypes.FixedResourceAndPulse)
            {
                float gainAmount = Mathf.Clamp01(AssetData.RestoreResourceValue);
                if (gainAmount > 0f)
                {
                    bool restored = false;
                    
                    if (Owner.MyVital.Mana != null)
                    {
                        LogInfo("공격독립체에서 마나 게이지를 증가시킵니다. {0}, {1:F2}", AssetData.ResourceConsumeType, gainAmount);
                        Owner.MyVital.Mana.OnAttackSuccess(gainAmount);
                        restored = true;
                    }
                    
                    if (Owner.MyVital.Pulse != null)
                    {
                        LogInfo("공격독립체에서 펄스 게이지를 증가시킵니다. {0}, {1:F2}", AssetData.ResourceConsumeType, gainAmount);
                        Owner.MyVital.Pulse.OnAttackSuccess(gainAmount);
                        restored = true;
                    }
                    
                    return restored;
                }
                return false;
            }

            // Life나 Barrier는 기존 방식 유지
            int value = (int)AssetData.RestoreResourceValue;

            if (value > 0)
            {
                LogInfo("공격독립체에서 자원을 회복합니다. {0}, {1}", AssetData.ResourceConsumeType, value);
                Owner.MyVital.AddCurrentValue(AssetData.ResourceConsumeType, value);

                return true;
            }

            return false;
        }
    }
}