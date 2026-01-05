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
                LogInfo("공격독립체에서 자원을 회복합니다. {0}, {1}", AssetData.ResourceConsumeType, value);
                Owner.MyVital.AddCurrentValue(AssetData.ResourceConsumeType, value);

                return true;
            }

            return false;
        }
    }
}